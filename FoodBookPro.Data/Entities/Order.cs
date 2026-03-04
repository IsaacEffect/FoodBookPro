namespace FoodBookPro.Data.Entities;

/// <summary>
/// Orden de un cliente (XAV-176)
/// </summary>
public class Order
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoOrden Estado { get; set; }
    public string RestauranteNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }

    /// <summary>
    /// Hora prevista de retiro del pedido (criterio XAV-194: filtros por hora de retiro)
    /// </summary>
    public DateTime? HoraRetiro { get; set; }

    /// <summary>
    /// Momento en que se inicio la preparacion (criterio XAV-197: timer de preparacion)
    /// </summary>
    public DateTime? FechaPreparacionInicio { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
