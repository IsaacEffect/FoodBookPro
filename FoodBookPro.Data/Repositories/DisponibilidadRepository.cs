using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DisponibilidadRepository : IDisponibilidadRepository
{
    private readonly List<DisponibilidadRestaurante> _disponibilidades = new();

    public Task<DisponibilidadRestaurante?> ObtenerPorDiaAsync(DayOfWeek diaSemana)
    {
        var resultado = _disponibilidades.FirstOrDefault(d => d.DiaSemana == diaSemana);
        return Task.FromResult(resultado);
    }

    public Task<IEnumerable<DisponibilidadRestaurante>> ObtenerTodasAsync()
    {
        return Task.FromResult(_disponibilidades.AsEnumerable());
    }

    public Task<DisponibilidadRestaurante> CrearAsync(DisponibilidadRestaurante disponibilidad)
    {
        if (_disponibilidades.Any(d => d.DiaSemana == disponibilidad.DiaSemana))
            throw new InvalidOperationException(
                $"Ya existe una configuración para el día {disponibilidad.DiaSemana}.");

        _disponibilidades.Add(disponibilidad);
        return Task.FromResult(disponibilidad);
    }

    public Task<DisponibilidadRestaurante> ActualizarAsync(DisponibilidadRestaurante disponibilidad)
    {
        var index = _disponibilidades.FindIndex(d => d.Id == disponibilidad.Id);
        if (index == -1)
            throw new KeyNotFoundException(
                $"No se encontró la configuración de disponibilidad con Id {disponibilidad.Id}.");

        _disponibilidades[index] = disponibilidad;
        return Task.FromResult(disponibilidad);
    }
}
