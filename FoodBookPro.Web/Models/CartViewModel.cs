namespace FoodBookPro.Web.Models;

/// <summary>
/// Vista principal del carrito con la lista de items y el total
/// </summary>
public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();

    // Total general del carrito
    public decimal Total => Items.Sum(i => i.Subtotal);
}
