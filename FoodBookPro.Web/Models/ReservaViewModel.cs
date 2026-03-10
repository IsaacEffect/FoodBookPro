using System;

public class ReservaViewModel
{
    public Guid Id { get; set; }
    public string CodigoReserva { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public int NumeroDPersonas { get; set; }
    public string? ComentariosEspeciales { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}