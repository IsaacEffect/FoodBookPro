namespace FoodBookPro.Web.Models;

/// <summary>
/// Item en el carrito para reordenar
/// </summary>
public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public decimal Subtotal => Price * Quantity;
}
