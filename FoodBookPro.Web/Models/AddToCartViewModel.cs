namespace FoodBookPro.Web.Models;

/// <summary>
/// Modelo para agregar un producto al carrito
/// </summary>
public class AddToCartViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}
