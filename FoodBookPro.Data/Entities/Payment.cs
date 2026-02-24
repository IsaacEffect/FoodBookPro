using System;

namespace FoodBookPro.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Para vincularlo al resumen de orden

        public decimal Subtotal { get; set; }
        public decimal TipAmount { get; set; } // Criterio: Opción de propina

        // Propiedad calculada: No necesita inicializarse porque depende de las anteriores
        public decimal TotalAmount => Subtotal + TipAmount;

        // Se inicializan con string.Empty para evitar el error de DbUpdateException (image_8924fe)
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }

        public bool IsSuccess { get; set; } // Criterio: Confirmación exitosa/fallida
        public string ErrorMessage { get; set; } = string.Empty; // Criterio: Manejo de errores
    }
}