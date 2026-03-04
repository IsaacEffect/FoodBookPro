using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodBookPro.Data.Entities;
using FoodBookPro.Data.Interfaces;

namespace FoodBookPro.Data.Repositories;

public class ReservaRepository : IReservaRepository
{
    // Almacenamiento en memoria — lista compartida por toda la instancia
    private readonly List<Reserva> _reservas = new();

    // ── CRUD básico ──────────────────────────────────────────────────

    public Task<Reserva> CrearAsync(Reserva reserva)
    {
        _reservas.Add(reserva);
        return Task.FromResult(reserva);
    }

    public Task<Reserva?> ObtenerPorIdAsync(Guid id)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(reserva);
    }

    public Task<Reserva?> ObtenerPorCodigoAsync(string codigoReserva)
    {
        var reserva = _reservas.FirstOrDefault(r => r.CodigoReserva == codigoReserva);
        return Task.FromResult(reserva);
    }

    public Task<Reserva> ModificarAsync(Reserva reserva)
    {
        var index = _reservas.FindIndex(r => r.Id == reserva.Id);
        if (index == -1)
            throw new KeyNotFoundException($"No se encontró la reserva con Id {reserva.Id}.");

        _reservas[index] = reserva;
        return Task.FromResult(reserva);
    }

    public Task EliminarAsync(Guid id)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva con Id {id}.");

        _reservas.Remove(reserva);
        return Task.CompletedTask;
    }

    // ── Consultas ────────────────────────────────────────────────────

    public Task<IEnumerable<Reserva>> ObtenerPorClienteAsync(Guid clienteId)
    {
        var resultado = _reservas
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.FechaHora)
            .AsEnumerable();

        return Task.FromResult(resultado);
    }

    public Task<IEnumerable<Reserva>> ObtenerTodasAsync(
        DateTime? desde = null,
        DateTime? hasta = null,
        EstadoReserva? estado = null)
    {
        IEnumerable<Reserva> query = _reservas;

        if (desde.HasValue)
            query = query.Where(r => r.FechaHora >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(r => r.FechaHora <= hasta.Value);

        if (estado.HasValue)
            query = query.Where(r => r.Estado == estado.Value);

        return Task.FromResult(query.OrderBy(r => r.FechaHora).AsEnumerable());
    }

    // ── Gestión de estado ────────────────────────────────────────────

    public Task<Reserva> ConfirmarAsync(Guid id)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva con Id {id}.");

        reserva.Confirmar();
        return Task.FromResult(reserva);
    }

    public Task<Reserva> RechazarAsync(Guid id)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva con Id {id}.");

        reserva.Rechazar();
        return Task.FromResult(reserva);
    }

    public Task<Reserva> CancelarAsync(Guid id)
    {
        var reserva = _reservas.FirstOrDefault(r => r.Id == id)
            ?? throw new KeyNotFoundException($"No se encontró la reserva con Id {id}.");

        reserva.Cancelar();
        return Task.FromResult(reserva);
    }

    // ── Disponibilidad ───────────────────────────────────────────────

    public Task<bool> VerificarDisponibilidadAsync(DateTime fechaHora, int numeroDPersonas)
    {
        // Cuenta las reservas activas (Pendiente o Confirmada) en la misma franja horaria (±1 hora)
        var franjaInicio = fechaHora.AddHours(-1);
        var franjaFin = fechaHora.AddHours(1);

        var reservasEnFranja = _reservas
            .Where(r =>
                r.FechaHora >= franjaInicio &&
                r.FechaHora <= franjaFin &&
                (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada))
            .ToList();

        var personasOcupadas = reservasEnFranja.Sum(r => r.NumeroDPersonas);

        // Capacidad máxima configurable (puede inyectarse vía IDisponibilidadRepository en un servicio)
        const int CapacidadMaxima = 50;
        var hayDisponibilidad = (personasOcupadas + numeroDPersonas) <= CapacidadMaxima;

        return Task.FromResult(hayDisponibilidad);
    }
}
