namespace FoodBookPro.Data.Entities;

/// <summary>
/// Estados posibles de una orden segun criterios de aceptacion
/// </summary>
public enum EstadoOrden
{
    Pendiente = 0,
    Confirmada = 1,
    Preparando = 2,
    Lista = 3,
    Completada = 4,
    Cancelada = 5
}
