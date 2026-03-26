using FoodBookPro.Data.Entities;

namespace FoodBookPro.Web.Services;

/// <summary>Punto de extensión para notificaciones de confirmación (email/SMS); por defecto no-op.</summary>
public interface IAdvanceOrderNotifier
{
    void NotifyPlaced(Order order);
}

public sealed class NoOpAdvanceOrderNotifier : IAdvanceOrderNotifier
{
    public void NotifyPlaced(Order order) { }
}
