namespace FoodBookPro.Data.Entities;

/// <summary>
/// Historial de cambios de estado de una orden (XAV-177)
/// </summary>
public class OrderStatusHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public EstadoOrden EstadoAnterior { get; set; }
    public EstadoOrden EstadoNuevo { get; set; }
    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool NotificadoCliente { get; set; }

    public Order Order { get; set; } = null!;
}
