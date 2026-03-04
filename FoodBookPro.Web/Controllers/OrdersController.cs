using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web.Controllers;

/// <summary>
/// Controlador para ver mis ordenes (cliente)
/// </summary>
public class OrdersController : Controller
{
    private readonly FoodbookDbContext _db;

    public OrdersController(FoodbookDbContext db)
    {
        _db = db;
    }

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

    public async Task<IActionResult> Detail(int id)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        return View(order);
    }
}
