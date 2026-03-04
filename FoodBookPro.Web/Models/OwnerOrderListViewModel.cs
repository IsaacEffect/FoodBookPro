using FoodBookPro.Data.Entities;

namespace FoodBookPro.Web.Models;

/// <summary>
/// Vista para el panel de ordenes del propietario (XAV-176)
/// Criterio: Panel de ordenes en tiempo real, filtros por estado y hora de retiro
/// </summary>
public class OwnerOrderListViewModel
{
    public List<OwnerOrderListItemViewModel> Orders { get; set; } = new();

    public EstadoOrden? FiltroEstado { get; set; }
    public DateTime? FiltroHoraRetiroDesde { get; set; }
    public DateTime? FiltroHoraRetiroHasta { get; set; }
}

public class OwnerOrderListItemViewModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public EstadoOrden Estado { get; set; }
    public string RestauranteNombre { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public DateTime? HoraRetiro { get; set; }
    public DateTime? FechaPreparacionInicio { get; set; }
}
