using System;

namespace FoodBookPro.Data.Entities;

public class DisponibilidadRestaurante
{
    // 1. Cambiamos Guid a int para que coincida con los IDs del test
    public int Id { get; set; }

    // 2. Agregamos RestauranteId, que es vital para las relaciones
    public int RestauranteId { get; set; }

    // 3. Renombramos para cumplir con CS0117 y CS1061
    public int CapacidadMaxima { get; set; } // Antes CapacidadMaximaPersonas
    public int LugaresOcupados { get; set; } // Propiedad faltante que pide el test

    // 4. Hacemos los set públicos para que el test pueda preparar el escenario
    public DayOfWeek DiaSemana { get; set; }
    public TimeSpan HoraApertura { get; set; }
    public TimeSpan HoraCierre { get; set; }
    public bool ConfirmacionInmediata { get; set; }
    public bool EstaDisponible { get; set; }

    // 5. El constructor DEBE ser público para corregir el error CS0122
    public DisponibilidadRestaurante() { }

    // Mantenemos tu lógica de negocio adaptada
    public static DisponibilidadRestaurante Crear(
        int restauranteId,
        DayOfWeek diaSemana,
        TimeSpan apertura,
        TimeSpan cierre,
        int capacidad,
        bool inmediata)
    {
        return new DisponibilidadRestaurante
        {
            RestauranteId = restauranteId,
            DiaSemana = diaSemana,
            HoraApertura = apertura,
            HoraCierre = cierre,
            CapacidadMaxima = capacidad,
            LugaresOcupados = 0,
            ConfirmacionInmediata = inmediata,
            EstaDisponible = true
        };
    }

    public bool EstaEnHorario(TimeSpan hora) =>
        EstaDisponible && hora >= HoraApertura && hora <= HoraCierre;
}