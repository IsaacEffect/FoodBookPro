using System.Text.Json;
using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web.Controllers;

/// <summary>
/// Controlador para ver mis ordenes (XAV-33)
/// </summary>
public class OrdersController : Controller
{
    private readonly FoodbookDbContext _db;

    public OrdersController(FoodbookDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Listado de ordenes con filtros opcionales
    /// </summary>
    public async Task<IActionResult> Index(
        EstadoOrden? estado,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        string? restaurante)
    {
        var query = _db.Orders.AsQueryable();

        if (estado.HasValue)
            query = query.Where(o => o.Estado == estado.Value);

        if (fechaDesde.HasValue)
            query = query.Where(o => o.Fecha.Date >= fechaDesde.Value.Date);

        if (fechaHasta.HasValue)
            query = query.Where(o => o.Fecha.Date <= fechaHasta.Value.Date);

        if (!string.IsNullOrWhiteSpace(restaurante))
            query = query.Where(o => o.RestauranteNombre.Contains(restaurante));

        var orders = await query
            .OrderByDescending(o => o.Fecha)
            .Select(o => new OrderListItemViewModel
            {
                Id = o.Id,
                Fecha = o.Fecha,
                Estado = o.Estado,
                RestauranteNombre = o.RestauranteNombre,
                Total = o.Total
            })
            .ToListAsync();

        var model = new OrderListViewModel
        {
            Orders = orders,
            FiltroEstado = estado,
            FiltroFechaDesde = fechaDesde,
            FiltroFechaHasta = fechaHasta,
            FiltroRestaurante = restaurante
        };

        return View(model);
    }

    /// <summary>
    /// Detalle de una orden
    /// </summary>
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
    /// Actualiza el estado de una orden (XAV-202)
    /// Transiciones validas: Pendiente->Cancelada (cliente), otras para el negocio
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, EstadoOrden nuevoEstado)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        var transicionValida = order.Estado switch
        {
            EstadoOrden.Pendiente => nuevoEstado == EstadoOrden.Cancelada,
            EstadoOrden.Confirmada => nuevoEstado is EstadoOrden.Preparando or EstadoOrden.Cancelada,
            EstadoOrden.Preparando => nuevoEstado == EstadoOrden.Lista,
            EstadoOrden.Lista => nuevoEstado == EstadoOrden.Completada,
            _ => false
        };

        if (!transicionValida)
            return BadRequest("Transicion de estado no permitida");

        order.Estado = nuevoEstado;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Detail), new { id });
    }

    /// <summary>
    /// Reordenar: copia items de la orden al carrito (XAV-191)
    /// </summary>
    public async Task<IActionResult> Reorder(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();

        var items = order.Items.Select((oi, i) => new CartItemViewModel
        {
            ProductId = i + 1,
            Name = oi.ProductoNombre,
            Price = oi.Precio,
            Quantity = oi.Cantidad
        }).ToList();

        var json = JsonSerializer.Serialize(items);
        HttpContext.Session.SetString(CartController.CartSessionKey, json);
        HttpContext.Session.SetString(CartController.RestaurantSessionKey, order.RestauranteNombre);

        return RedirectToAction("Index", "Cart");
    }

    /// <summary>
    /// Descarga el comprobante de la orden (XAV-190)
    /// </summary>
    public async Task<IActionResult> DownloadReceipt(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();

        var html = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'><title>Comprobante Orden #{order.Id}</title></head>
<body style='font-family: sans-serif; padding: 20px;'>
<h1>Comprobante de Orden</h1>
<p><strong>Orden #</strong>{order.Id}</p>
<p><strong>Fecha:</strong> {order.Fecha:dd/MM/yyyy HH:mm}</p>
<p><strong>Restaurante:</strong> {order.RestauranteNombre}</p>
<p><strong>Estado:</strong> {order.Estado}</p>
<hr/>
<table style='width:100%; border-collapse: collapse;'>
<tr style='background:#eee'><th>Producto</th><th>Cant.</th><th>Precio</th><th>Subtotal</th></tr>
{string.Join("", order.Items.Select(i => $"<tr><td>{i.ProductoNombre}</td><td>{i.Cantidad}</td><td>{i.Precio:C}</td><td>{i.Subtotal:C}</td></tr>"))}
</table>
<p style='font-size:1.2em; margin-top:20px'><strong>Total: {order.Total:C}</strong></p>
<p style='margin-top:40px; color:#666; font-size:0.9em'>FoodBookPro - Comprobante generado el {DateTime.Now:dd/MM/yyyy HH:mm}</p>
</body>
</html>";

        var bytes = System.Text.Encoding.UTF8.GetBytes(html);
        return File(bytes, "text/html", $"comprobante-orden-{order.Id}.html");
    }
}
