using System;

public class Reserva
{
    public Guid Id { get; private set; }
    public string CodigoReserva { get; private set; }
    public Guid ClienteId { get; private set; }
    public string NombreCliente { get; private set; }
    public string EmailCliente { get; private set; }
    public string TelefonoCliente { get; private set; }
    public DateTime FechaHora { get; private set; }
    public int NumeroDPersonas { get; private set; }
    public string? ComentariosEspeciales { get; private set; }
    public EstadoReserva Estado { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime? FechaModificacion { get; private set; }

    // Constructor privado para controlar la creación
    private Reserva() 
    {
        CodigoReserva = string.Empty;
        NombreCliente = string.Empty;
        EmailCliente = string.Empty;
        TelefonoCliente = string.Empty;
    }

    public static Reserva Crear(
        Guid clienteId,
        string nombreCliente,
        string emailCliente,
        string telefonoCliente,
        DateTime fechaHora,
        int numeroDPersonas,
        bool confirmacionInmediata,
        string? comentariosEspeciales = null)
    {
        return new Reserva
        {
            Id = Guid.NewGuid(),
            CodigoReserva = GenerarCodigo(),
            ClienteId = clienteId,
            NombreCliente = nombreCliente,
            EmailCliente = emailCliente,
            TelefonoCliente = telefonoCliente,
            FechaHora = fechaHora,
            NumeroDPersonas = numeroDPersonas,
            ComentariosEspeciales = comentariosEspeciales,
            Estado = confirmacionInmediata ? EstadoReserva.Confirmada : EstadoReserva.Pendiente,
            FechaCreacion = DateTime.UtcNow,
            FechaModificacion = null
        };
    }

    public void Modificar(
        DateTime fechaHora,
        int numeroDPersonas,
        string? comentariosEspeciales)
    {
        FechaHora = fechaHora;
        NumeroDPersonas = numeroDPersonas;
        ComentariosEspeciales = comentariosEspeciales;
        FechaModificacion = DateTime.UtcNow;
    }

    public void Confirmar()
    {
        Estado = EstadoReserva.Confirmada;
        FechaModificacion = DateTime.UtcNow;
    }

    public void Rechazar()
    {
        Estado = EstadoReserva.Rechazada;
        FechaModificacion = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Estado = EstadoReserva.Cancelada;
        FechaModificacion = DateTime.UtcNow;
    }

    private static string GenerarCodigo()
    {
        return $"RES-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }
}
