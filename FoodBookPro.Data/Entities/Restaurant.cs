using System;

namespace FoodBookPro.Data.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // --- Campos de la XAV-34 ---
        public string CuisineType { get; set; } = string.Empty;
        public string PriceRange { get; set; } = "$";
        public double Rating { get; set; }
        public double Distance { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // --- NUEVAS COORDENADAS PARA XAV-35 ---
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}