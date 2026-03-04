using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodBookPro.Data.Entities;

namespace FoodBookPro.Data.Interfaces;

public interface IReservaRepository
{
    Task<Reserva> CrearAsync(Reserva reserva);
    Task<Reserva?> ObtenerPorIdAsync(Guid id);
    Task<Reserva?> ObtenerPorCodigoAsync(string codigoReserva);
    Task<Reserva> ModificarAsync(Reserva reserva);
    Task EliminarAsync(Guid id);

    /// Devuelve todas las reservas de un cliente autenticado.
    Task<IEnumerable<Reserva>> ObtenerPorClienteAsync(Guid clienteId);

    /// Devuelve todas las reservas del restaurante, con filtros opcionales.
    Task<IEnumerable<Reserva>> ObtenerTodasAsync(
        DateTime? desde = null,
        DateTime? hasta = null,
        EstadoReserva? estado = null);

    // Gestión de estado 
    Task<Reserva> ConfirmarAsync(Guid id);
    Task<Reserva> RechazarAsync(Guid id);
    Task<Reserva> CancelarAsync(Guid id);

    /// Indica si hay cupo disponible para la fecha/hora y número de personas.
    Task<bool> VerificarDisponibilidadAsync(DateTime fechaHora, int numeroDPersonas);
}
