using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodBookPro.Data.Entities;
public interface IDisponibilidadRepository
{
    Task<DisponibilidadRestaurante?> ObtenerPorDiaAsync(DayOfWeek diaSemana);
    Task<IEnumerable<DisponibilidadRestaurante>> ObtenerTodasAsync();
    Task<DisponibilidadRestaurante> CrearAsync(DisponibilidadRestaurante disponibilidad);
    Task<DisponibilidadRestaurante> ActualizarAsync(DisponibilidadRestaurante disponibilidad);
}
