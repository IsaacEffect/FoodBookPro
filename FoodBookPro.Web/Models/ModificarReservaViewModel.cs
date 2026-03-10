using System;

public class ModificarReservaViewModel
{
    public Guid Id { get; set; }
    public string CodigoReserva { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
    [Display(Name = "Fecha y hora")]
    public DateTime FechaHora { get; set; }

    [Required(ErrorMessage = "El número de personas es obligatorio.")]
    [Range(1, 20, ErrorMessage = "El número de personas debe ser entre 1 y 20.")]
    [Display(Name = "Número de personas")]
    public int NumeroDPersonas { get; set; }

    [MaxLength(500)]
    [Display(Name = "Comentarios especiales")]
    public string? ComentariosEspeciales { get; set; }
}
