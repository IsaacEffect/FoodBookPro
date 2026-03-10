using System;

public class CrearReservaViewModel
{
    [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
    [Display(Name = "Fecha y hora")]
    public DateTime FechaHora { get; set; } = DateTime.Now.AddDays(1);

    [Required(ErrorMessage = "El número de personas es obligatorio.")]
    [Range(1, 20, ErrorMessage = "El número de personas debe ser entre 1 y 20.")]
    [Display(Name = "Número de personas")]
    public int NumeroDPersonas { get; set; } = 2;

    [MaxLength(500, ErrorMessage = "Los comentarios no pueden superar los 500 caracteres.")]
    [Display(Name = "Comentarios especiales")]
    public string? ComentariosEspeciales { get; set; }
}
