using System.Text.Json;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FoodBookPro.Web.Controllers;

/// <summary>
/// Controlador del carrito de ordenes
/// Usa sesion para almacenar los items (simple, sin base de datos)
/// </summary>
public class CartController : Controller
{
    private const string CartSessionKey = "Cart";

    // Lista de productos de ejemplo para poder agregar al carrito
    private static readonly List<CartItemViewModel> SampleProducts = new()
    {
        new CartItemViewModel { ProductId = 1, Name = "Hamburguesa", Price = 8.99m },
        new CartItemViewModel { ProductId = 2, Name = "Pizza", Price = 12.50m },
        new CartItemViewModel { ProductId = 3, Name = "Ensalada", Price = 6.00m }
    };

    /// <summary>
    /// Muestra el carrito actual
    /// </summary>
    public IActionResult Index()
    {
        var items = GetCartFromSession();
        var model = new CartViewModel { Items = items };
        return View(model);
    }

    /// <summary>
    /// Pagina para elegir un producto y agregarlo al carrito
    /// </summary>
    public IActionResult Add()
    {
        return View(SampleProducts);
    }

    /// <summary>
    /// Agrega un producto al carrito y redirige al carrito
    /// </summary>
    [HttpPost]
    public IActionResult AddItem(int productId, int quantity)
    {
        var product = SampleProducts.FirstOrDefault(p => p.ProductId == productId);
        if (product == null)
            return RedirectToAction(nameof(Add));

        var cart = GetCartFromSession();
        var existing = cart.FirstOrDefault(i => i.ProductId == productId);

        if (existing != null)
            existing.Quantity += quantity;
        else
            cart.Add(new CartItemViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            });

        SaveCartToSession(cart);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Actualiza la cantidad de un item en el carrito
    /// </summary>
    [HttpPost]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
            return RedirectToAction(nameof(Index));

        if (quantity <= 0)
            cart.Remove(item);
        else
            item.Quantity = quantity;

        SaveCartToSession(cart);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Elimina un item del carrito
    /// </summary>
    [HttpPost]
    public IActionResult Remove(int productId)
    {
        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
            cart.Remove(item);

        SaveCartToSession(cart);
        return RedirectToAction(nameof(Index));
    }

    private List<CartItemViewModel> GetCartFromSession()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json))
            return new List<CartItemViewModel>();

        try
        {
            return JsonSerializer.Deserialize<List<CartItemViewModel>>(json) ?? new List<CartItemViewModel>();
        }
        catch
        {
            return new List<CartItemViewModel>();
        }
    }

    private void SaveCartToSession(List<CartItemViewModel> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString(CartSessionKey, json);
    }
}
