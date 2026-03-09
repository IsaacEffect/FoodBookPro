using System;

namespace FoodBookPro.Data.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // Mantenemos de develop
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty; // Mantenemos de develop

        // --- Campos obligatorios para XAV-34 ---
        public string CuisineType { get; set; } = string.Empty; // Especialidad
        public string PriceRange { get; set; } = "$";           // Rango: $, $$, $$$
        public double Rating { get; set; }                     // Calificación
        public double Distance { get; set; }                   // Distancia en km

        public string ImageUrl { get; set; } = string.Empty;
    }
}