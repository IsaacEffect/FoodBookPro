using System.Text.Json;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodBookPro.Web.Controllers;

/// <summary>
/// Carrito para reordenar desde historial (XAV-191)
/// </summary>
public class CartController : Controller
{
    public const string CartSessionKey = "ReorderCart";
    public const string RestaurantSessionKey = "ReorderRestaurant";

    public IActionResult Index()
    {
        var items = GetCartFromSession();
        var restaurant = HttpContext.Session.GetString(RestaurantSessionKey) ?? "";
        var model = new CartViewModel { Items = items, RestauranteNombre = restaurant };
        return View(model);
    }

    private List<CartItemViewModel> GetCartFromSession()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItemViewModel>();
        try
        {
            return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>();
        }
        catch
        {
            return new List<CartItemViewModel>();
        }
    }
}
