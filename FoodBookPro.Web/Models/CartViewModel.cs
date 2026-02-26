namespace FoodBookPro.Web.Models;

/// <summary>
/// Vista del carrito de reorden
/// </summary>
public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public string RestauranteNombre { get; set; } = string.Empty;

    public decimal Total => Items.Sum(i => i.Subtotal);
}
