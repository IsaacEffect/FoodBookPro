using System.Text.Json;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodBookPro.Web.Controllers;

/// <summary>XAV-31 carrito en sesión; compatible con Reordenar (Orders).</summary>
public class CartController : Controller
{
    public const string CartSessionKey = "ReorderCart";
    public const string RestaurantSessionKey = "ReorderRestaurant";

    public IActionResult Index()
    {
        var items = LoadCart();
        var restaurant = HttpContext.Session.GetString(RestaurantSessionKey) ?? "";
        return View(new CartViewModel { Items = items, RestauranteNombre = restaurant });
    }

    [HttpGet]
    public IActionResult Add()
    {
        var catalog = new List<CartItemViewModel>
        {
            new() { ProductId = 1, Name = "Pizza Margarita", Price = 12.99m, Quantity = 1 },
            new() { ProductId = 2, Name = "Hamburguesa", Price = 8.50m, Quantity = 1 },
            new() { ProductId = 3, Name = "Refresco", Price = 2.00m, Quantity = 1 }
        };
        return View(catalog);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddItem(int productId, string name, decimal price, int quantity)
    {
        if (quantity < 1) quantity = 1;
        var list = LoadCart();
        var ex = list.FirstOrDefault(i => i.ProductId == productId);
        if (ex != null) ex.Quantity += quantity;
        else list.Add(new CartItemViewModel { ProductId = productId, Name = name ?? "", Price = price, Quantity = quantity });
        SaveCart(list);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int index)
    {
        var list = LoadCart();
        if (index >= 0 && index < list.Count) list.RemoveAt(index);
        SaveCart(list);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int index, int quantity)
    {
        var list = LoadCart();
        if (index < 0 || index >= list.Count) return RedirectToAction(nameof(Index));
        if (quantity <= 0) list.RemoveAt(index);
        else list[index].Quantity = quantity;
        SaveCart(list);
        return RedirectToAction(nameof(Index));
    }

    private List<CartItemViewModel> LoadCart()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItemViewModel>();
        try { return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>(); }
        catch { return new List<CartItemViewModel>(); }
    }

    private void SaveCart(List<CartItemViewModel> items) =>
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(items));
}
