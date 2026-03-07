using System;

public class DisponibilidadRestaurante
{
    public Guid Id { get; private set; }
    public DayOfWeek DiaSemana { get; private set; }
    public TimeSpan HoraApertura { get; private set; }
    public TimeSpan HoraCierre { get; private set; }
    public int CapacidadMaximaPersonas { get; private set; }
    public int MaximaReservasPorFranja { get; private set; }
    public bool ConfirmacionInmediata { get; private set; }
    public bool EstaDisponible { get; private set; }

    private DisponibilidadRestaurante() { }

    public static DisponibilidadRestaurante Crear(
        DayOfWeek diaSemana,
        TimeSpan horaApertura,
        TimeSpan horaCierre,
        int capacidadMaximaPersonas,
        int maximaReservasPorFranja,
        bool confirmacionInmediata)
    {
        return new DisponibilidadRestaurante
        {
            Id = Guid.NewGuid(),
            DiaSemana = diaSemana,
            HoraApertura = horaApertura,
            HoraCierre = horaCierre,
            CapacidadMaximaPersonas = capacidadMaximaPersonas,
            MaximaReservasPorFranja = maximaReservasPorFranja,
            ConfirmacionInmediata = confirmacionInmediata,
            EstaDisponible = true
        };
    }

    public void Actualizar(
        TimeSpan horaApertura,
        TimeSpan horaCierre,
        int capacidadMaximaPersonas,
        int maximaReservasPorFranja,
        bool confirmacionInmediata,
        bool estaDisponible)
    {
        HoraApertura = horaApertura;
        HoraCierre = horaCierre;
        CapacidadMaximaPersonas = capacidadMaximaPersonas;
        MaximaReservasPorFranja = maximaReservasPorFranja;
        ConfirmacionInmediata = confirmacionInmediata;
        EstaDisponible = estaDisponible;
    }

    public bool EstaEnHorario(TimeSpan hora) =>
        EstaDisponible && hora >= HoraApertura && hora <= HoraCierre;
}

