using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web.Controllers
{
    /// <summary>
    /// Controlador para recibir y gestionar ordenes como propietario (XAV-176)
    /// User Story: Como propietario quiero recibir y gestionar las ordenes anticipadas para preparar los pedidos a tiempo
    /// </summary>
    public class OwnerOrdersController : Controller
    {
        private readonly FoodbookDbContext _db;

        public OwnerOrdersController(FoodbookDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Panel de ordenes en tiempo real (criterio: Panel de ordenes en tiempo real)
        /// Filtros por estado y hora de retiro (criterio XAV-194)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(
            EstadoOrden? estado,
            DateTime? horaRetiroDesde,
            DateTime? horaRetiroHasta)
        {
            var query = _db.Orders.AsQueryable();

            if (estado.HasValue)
                query = query.Where(o => o.Estado == estado.Value);

            if (horaRetiroDesde.HasValue)
                query = query.Where(o => o.HoraRetiro != null && o.HoraRetiro.Value >= horaRetiroDesde.Value);

            if (horaRetiroHasta.HasValue)
                query = query.Where(o => o.HoraRetiro != null && o.HoraRetiro.Value <= horaRetiroHasta.Value);

            var orders = await query
                .OrderByDescending(o => o.Fecha)
                .Select(o => new OwnerOrderListItemViewModel
                {
                    Id = o.Id,
                    Fecha = o.Fecha,
                    Estado = o.Estado,
                    RestauranteNombre = o.RestauranteNombre,
                    Total = o.Total,
                    HoraRetiro = o.HoraRetiro,
                    FechaPreparacionInicio = o.FechaPreparacionInicio
                })
                .ToListAsync();

            var model = new OwnerOrderListViewModel
            {
                Orders = orders,
                FiltroEstado = estado,
                FiltroHoraRetiroDesde = horaRetiroDesde,
                FiltroHoraRetiroHasta = horaRetiroHasta
            };

            return View(model);
        }

        /// <summary>
        /// Vista de detalles completos de cada orden (criterio XAV-193)
        /// Incluye botones de accion: Aceptar, Rechazar, Marcar Preparando, Marcar Lista (XAV-195)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        /// <summary>
        /// Vista de cocina optimizada (criterio XAV-196)
        /// Muestra ordenes en preparacion con timer
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Kitchen()
        {
            var orders = await _db.Orders
                .Include(o => o.Items)
                .Where(o => o.Estado == EstadoOrden.Confirmada || o.Estado == EstadoOrden.Preparando)
                .OrderBy(o => o.HoraRetiro ?? o.Fecha)
                .ToListAsync();

            return View(orders);
        }

        /// <summary>
        /// Actualiza el estado de una orden (criterio XAV-195: botones de accion)
        /// Acciones: Accept, Reject, Preparando, Lista
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string accion)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            var nuevoEstado = accion?.ToLowerInvariant() switch
            {
                "accept" or "aceptar" => EstadoOrden.Confirmada,
                "reject" or "rechazar" => EstadoOrden.Cancelada,
                "preparando" => EstadoOrden.Preparando,
                "lista" => EstadoOrden.Lista,
                _ => (EstadoOrden?)null
            };

            if (!nuevoEstado.HasValue)
                return BadRequest("Accion no valida");

            order.Estado = nuevoEstado.Value;

            if (nuevoEstado == EstadoOrden.Preparando && !order.FechaPreparacionInicio.HasValue)
            {
                order.FechaPreparacionInicio = DateTime.Now;
            }

            await _db.SaveChangesAsync();

            if (Request != null && string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal))
                return Json(new { success = true, estado = order.Estado.ToString() });

            return RedirectToAction(nameof(Detail), new { id });
        }

        /// <summary>
        /// Endpoint para polling: devuelve conteo de ordenes pendientes (para notificacion de nueva orden)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPendingCount()
        {
            var count = await _db.Orders.CountAsync(o => o.Estado == EstadoOrden.Pendiente);
            return Json(new { count });
        }
    }
}
