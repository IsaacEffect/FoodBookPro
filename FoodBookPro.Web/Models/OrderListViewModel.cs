using FoodBookPro.Data.Entities;

namespace FoodBookPro.Web.Models;

/// <summary>
/// Vista para el listado de ordenes con filtros
/// </summary>
public class OrderListViewModel
{
    public List<OrderListItemViewModel> Orders { get; set; } = new();

    // Filtros aplicados
    public EstadoOrden? FiltroEstado { get; set; }
    public DateTime? FiltroFechaDesde { get; set; }
    public DateTime? FiltroFechaHasta { get; set; }
    public string? FiltroRestaurante { get; set; }
}

public class OrderListItemViewModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoOrden Estado { get; set; }
    public string RestauranteNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
