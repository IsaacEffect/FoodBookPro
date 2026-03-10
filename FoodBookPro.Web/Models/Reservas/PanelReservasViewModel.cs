using System;
using System.Collections.Generic;
using System.Linq;


// ── Panel del restaurante ────────────────────────────────────────────
public class PanelReservasViewModel
{
    public IEnumerable<ReservaViewModel> Reservas { get; set; } = Enumerable.Empty<ReservaViewModel>();
    public DateTime? FiltroDesde { get; set; }
    public DateTime? FiltroHasta { get; set; }
    public string? FiltroEstado { get; set; }

    public int TotalReservas => Reservas.Count();
    public int TotalPendientes => Reservas.Count(r => r.Estado == "Pendiente");
    public int TotalConfirmadas => Reservas.Count(r => r.Estado == "Confirmada");
}

// ── Disponibilidad ───────────────────────────────────────────────────
public class DisponibilidadViewModel
{
    public Guid Id { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public TimeSpan HoraApertura { get; set; }
    public TimeSpan HoraCierre { get; set; }
    public int CapacidadMaximaPersonas { get; set; }
    public int MaximaReservasPorFranja { get; set; }
    public bool ConfirmacionInmediata { get; set; }
    public bool EstaDisponible { get; set; }
}

public class ConfigurarDisponibilidadViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Día de la semana")]
    public string DiaSemana { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Hora de apertura")]
    public TimeSpan HoraApertura { get; set; }

    [Required]
    [Display(Name = "Hora de cierre")]
    public TimeSpan HoraCierre { get; set; }

    [Required]
    [Range(1, 500)]
    [Display(Name = "Capacidad máxima (personas)")]
    public int CapacidadMaximaPersonas { get; set; }

    [Required]
    [Range(1, 100)]
    [Display(Name = "Máx. reservas por franja")]
    public int MaximaReservasPorFranja { get; set; }

    [Display(Name = "Confirmación inmediata")]
    public bool ConfirmacionInmediata { get; set; }

    [Display(Name = "Disponible")]
    public bool EstaDisponible { get; set; }

    public IEnumerable<DisponibilidadViewModel> Configuraciones { get; set; }
        = Enumerable.Empty<DisponibilidadViewModel>();
}