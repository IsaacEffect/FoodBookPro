namespace FoodBookPro.Web.Services;

/// <summary>XAV-30: validación mínima de horario y catálogo para orden anticipada.</summary>
public static class AdvanceOrderRules
{
    public static readonly HashSet<string> ProductosCatalogo = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pizza Margarita", "Hamburguesa", "Refresco", "Roll Sushi"
    };

    public static string? ValidateSchedule(DateTime horaRetiro, DateTime ahora)
    {
        if (horaRetiro <= ahora.AddMinutes(30))
            return "El retiro debe ser al menos 30 minutos desde ahora.";
        if (horaRetiro > ahora.AddDays(14))
            return "No puede pedir con más de 14 días de anticipación.";
        if (horaRetiro.Hour < 10 || horaRetiro.Hour >= 22)
            return "Horario de retiro entre 10:00 y 21:59.";
        return null;
    }

    public static string? ValidateCartProductNames(IEnumerable<string> nombres)
    {
        foreach (var n in nombres)
        {
            if (string.IsNullOrWhiteSpace(n)) return "Producto inválido.";
            if (!ProductosCatalogo.Contains(n.Trim()))
                return $"Producto no disponible para orden anticipada: {n}.";
        }
        return null;
    }
}
