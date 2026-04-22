using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web.Controllers;

/// <summary>XAV-176 panel/cocina/filtros; XAV-177 cambio estado, historial, tiempo estimado.</summary>
public class OwnerOrdersController : Controller
{
    private readonly FoodbookDbContext _db;

    public OwnerOrdersController(FoodbookDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(EstadoOrden? estado, DateTime? horaRetiroDesde, DateTime? horaRetiroHasta)
    {
        var query = _db.Orders.AsQueryable();
        if (estado.HasValue) query = query.Where(o => o.Estado == estado.Value);
        if (horaRetiroDesde.HasValue) query = query.Where(o => o.HoraRetiro != null && o.HoraRetiro >= horaRetiroDesde);
        if (horaRetiroHasta.HasValue) query = query.Where(o => o.HoraRetiro != null && o.HoraRetiro <= horaRetiroHasta);

        var orders = await query.OrderByDescending(o => o.Fecha)
            .Select(o => new OwnerOrderListItemViewModel { Id = o.Id, Fecha = o.Fecha, Estado = o.Estado, RestauranteNombre = o.RestauranteNombre, Total = o.Total, HoraRetiro = o.HoraRetiro, FechaPreparacionInicio = o.FechaPreparacionInicio })
            .ToListAsync();

        return View(new OwnerOrderListViewModel { Orders = orders, FiltroEstado = estado, FiltroHoraRetiroDesde = horaRetiroDesde, FiltroHoraRetiroHasta = horaRetiroHasta });
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Kitchen()
    {
        var orders = await _db.Orders.Include(o => o.Items)
            .Where(o => o.Estado == EstadoOrden.Confirmada || o.Estado == EstadoOrden.Preparando)
            .OrderBy(o => o.HoraRetiro ?? o.Fecha)
            .ToListAsync();
        return View(orders);
    }

    /// <summary>XAV-177: Cambio de estado con un clic, historial, comentarios, notificación.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string accion, string? comentario = null)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        var nuevoEstado = accion?.ToLowerInvariant() switch
        {
            "accept" or "aceptar" => EstadoOrden.Confirmada,
            "reject" or "rechazar" => EstadoOrden.Cancelada,
            "preparando" => EstadoOrden.Preparando,
            "lista" => EstadoOrden.Lista,
            _ => (EstadoOrden?)null
        };
        if (!nuevoEstado.HasValue) return BadRequest("Accion no valida");

        var anterior = order.Estado;
        order.Estado = nuevoEstado.Value;
        if (nuevoEstado == EstadoOrden.Preparando && !order.FechaPreparacionInicio.HasValue)
            order.FechaPreparacionInicio = DateTime.Now;

        _db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = id, EstadoAnterior = anterior, EstadoNuevo = nuevoEstado.Value,
            Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim(),
            NotificadoCliente = true
        });
        await _db.SaveChangesAsync();

        if (Request != null && string.Equals(Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal))
            return Json(new { success = true, estado = order.Estado.ToString() });
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfigurarTiempoEstimado(int id, int minutos)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();
        if (minutos < 1 || minutos > 480) return BadRequest("Minutos entre 1 y 480");
        order.TiempoEstimadoPreparacionMinutos = minutos;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> HistorialEstados(int id)
    {
        var historial = await _db.OrderStatusHistories.Where(h => h.OrderId == id).OrderByDescending(h => h.Fecha).ToListAsync();
        return Json(historial.Select(h => new { h.EstadoAnterior, h.EstadoNuevo, h.Comentario, h.Fecha, h.NotificadoCliente }));
    }

    [HttpGet]
    public async Task<IActionResult> GetPendingCount()
    {
        var count = await _db.Orders.CountAsync(o => o.Estado == EstadoOrden.Pendiente);
        return Json(new { count });
    }
}
