namespace FoodBookPro.Data.Entities;

/// <summary>
/// Item individual de una orden
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }

    public decimal Subtotal => Precio * Cantidad;

    public Order Order { get; set; } = null!;
}
