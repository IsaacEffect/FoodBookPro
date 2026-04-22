using System.Text.Json;
using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Models;
using FoodBookPro.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web.Controllers;

/// <summary>XAV-30: orden anticipada desde carrito (horario, catálogo, mismo flujo que OwnerOrders).</summary>
public class AdvanceOrderController : Controller
{
    private readonly FoodbookDbContext _db;
    private readonly IAdvanceOrderNotifier _notifier;

    public AdvanceOrderController(FoodbookDbContext db, IAdvanceOrderNotifier notifier)
    {
        _db = db;
        _notifier = notifier;
    }

    [HttpGet]
    public IActionResult Schedule()
    {
        var items = LoadCart();
        if (items.Count == 0) return RedirectToAction(nameof(CartController.Index), "Cart");
        var restaurant = HttpContext.Session.GetString(CartController.RestaurantSessionKey) ?? "";
        if (string.IsNullOrWhiteSpace(restaurant))
        {
            var first = _db.Restaurants.AsNoTracking().OrderBy(r => r.Name).Select(r => r.Name).FirstOrDefault();
            restaurant = first ?? "Pizza Palace";
        }

        return View(new AdvanceScheduleViewModel { Items = items, RestauranteNombre = restaurant });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Place(DateTime horaRetiro, string? customerName)
    {
        var items = LoadCart();
        if (items.Count == 0) return RedirectToAction(nameof(CartController.Index), "Cart");

        var restaurant = HttpContext.Session.GetString(CartController.RestaurantSessionKey)?.Trim();
        if (string.IsNullOrEmpty(restaurant))
        {
            restaurant = await _db.Restaurants.AsNoTracking().OrderBy(r => r.Name).Select(r => r.Name).FirstOrDefaultAsync()
                ?? "Pizza Palace";
        }

        var scheduleErr = AdvanceOrderRules.ValidateSchedule(horaRetiro, DateTime.Now);
        if (scheduleErr != null)
        {
            TempData["AdvanceOrderError"] = scheduleErr;
            return RedirectToAction(nameof(Schedule));
        }

        var productErr = AdvanceOrderRules.ValidateCartProductNames(items.Select(i => i.Name));
        if (productErr != null)
        {
            TempData["AdvanceOrderError"] = productErr;
            return RedirectToAction(nameof(Schedule));
        }

        var total = items.Sum(i => i.Subtotal);
        var order = new Order
        {
            Fecha = DateTime.Now,
            Estado = EstadoOrden.Pendiente,
            RestauranteNombre = restaurant,
            Total = total,
            CustomerName = string.IsNullOrWhiteSpace(customerName) ? "Cliente" : customerName.Trim(),
            HoraRetiro = horaRetiro
        };
        foreach (var i in items)
            order.Items.Add(new OrderItem { ProductoNombre = i.Name, Cantidad = i.Quantity, Precio = i.Price });

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        _notifier.NotifyPlaced(order);

        HttpContext.Session.Remove(CartController.CartSessionKey);
        HttpContext.Session.Remove(CartController.RestaurantSessionKey);

        TempData["AdvanceOrderOk"] =
            $"Tu orden anticipada #{order.Id} quedó pendiente. Retiro: {horaRetiro:dd/MM/yyyy HH:mm}.";
        return RedirectToAction(nameof(OrdersController.Detail), "Orders", new { id = order.Id });
    }

    private List<CartItemViewModel> LoadCart()
    {
        var json = HttpContext.Session.GetString(CartController.CartSessionKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItemViewModel>();
        try { return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>(); }
        catch { return new List<CartItemViewModel>(); }
    }
}
