using System.Text.Json;
using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web.Controllers;

/// <summary>Ver mis órdenes (XAV-33). Listado, detalle, reordenar, comprobante.</summary>
public class OrdersController : Controller
{
    private readonly FoodbookDbContext _db;

    public OrdersController(FoodbookDbContext db) => _db = db;

    public async Task<IActionResult> Index(EstadoOrden? estado, DateTime? fechaDesde, DateTime? fechaHasta, string? restaurante, bool soloAnticipadas = false)
    {
        var query = _db.Orders.AsQueryable();
        if (estado.HasValue) query = query.Where(o => o.Estado == estado.Value);
        if (fechaDesde.HasValue) query = query.Where(o => o.Fecha.Date >= fechaDesde.Value.Date);
        if (fechaHasta.HasValue) query = query.Where(o => o.Fecha.Date <= fechaHasta.Value.Date);
        if (!string.IsNullOrWhiteSpace(restaurante)) query = query.Where(o => o.RestauranteNombre.Contains(restaurante));
        if (soloAnticipadas) query = query.Where(o => o.HoraRetiro != null);

        var orders = await query.OrderByDescending(o => o.Fecha)
            .Select(o => new OrderListItemViewModel { Id = o.Id, Fecha = o.Fecha, Estado = o.Estado, RestauranteNombre = o.RestauranteNombre, Total = o.Total, HoraRetiro = o.HoraRetiro })
            .ToListAsync();

        return View(new OrderListViewModel
        {
            Orders = orders,
            FiltroEstado = estado,
            FiltroFechaDesde = fechaDesde,
            FiltroFechaHasta = fechaHasta,
            FiltroRestaurante = restaurante,
            SoloAnticipadas = soloAnticipadas
        });
    }

    public async Task<IActionResult> Detail(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return View(order);
    }

    /// <summary>Cliente solo puede cancelar orden pendiente.</summary>
    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();
        if (order.Estado != EstadoOrden.Pendiente) return BadRequest("Solo se puede cancelar una orden pendiente.");
        order.Estado = EstadoOrden.Cancelada;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Detail), new { id });
    }

    public async Task<IActionResult> Reorder(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        var items = order.Items.Select((oi, i) => new CartItemViewModel { ProductId = i + 1, Name = oi.ProductoNombre, Price = oi.Precio, Quantity = oi.Cantidad }).ToList();
        HttpContext.Session.SetString(CartController.CartSessionKey, JsonSerializer.Serialize(items));
        HttpContext.Session.SetString(CartController.RestaurantSessionKey, order.RestauranteNombre);
        return RedirectToAction("Index", "Cart");
    }

    public async Task<IActionResult> DownloadReceipt(int id)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        var html = $@"<!DOCTYPE html><html><head><meta charset='utf-8'><title>Comprobante #{order.Id}</title></head><body style='font-family:sans-serif;padding:20px'>
<h1>Comprobante Orden #{order.Id}</h1><p>Fecha: {order.Fecha:dd/MM/yyyy HH:mm} | Restaurante: {order.RestauranteNombre} | Estado: {order.Estado}</p><hr/>
<table style='width:100%;border-collapse:collapse'><tr style='background:#eee'><th>Producto</th><th>Cant.</th><th>Precio</th><th>Subtotal</th></tr>
{string.Join("", order.Items.Select(i => $"<tr><td>{i.ProductoNombre}</td><td>{i.Cantidad}</td><td>{i.Precio:C}</td><td>{i.Subtotal:C}</td></tr>"))}
</table><p style='font-size:1.2em'><strong>Total: {order.Total:C}</strong></p></body></html>";
        return File(System.Text.Encoding.UTF8.GetBytes(html), "text/html", $"comprobante-{order.Id}.html");
    }
}
