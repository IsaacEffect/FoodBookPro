using System;

namespace FoodBookPro.Data.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Criterio: Búsqueda por nombre
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty; // Criterio: Búsqueda por ciudad/zona
        public string CuisineType { get; set; } = string.Empty; // Criterio: Tipo de cocina
        public string ImageUrl { get; set; } = string.Empty; // Criterio: Imagen en tarjeta
        public double Rating { get; set; } // Criterio: Calificación
        public string Address { get; set; } = string.Empty;
    }
}