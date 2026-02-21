using System;

namespace FoodBookPro.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // Para el resumen de orden

        public decimal Subtotal { get; set; }
        public decimal TipAmount { get; set; } // Criterio: Opción de propina
        public decimal TotalAmount => Subtotal + TipAmount; // Calculado

        public string PaymentMethod { get; set; } // Criterio: Selección de método
        public DateTime TransactionDate { get; set; }

        public bool IsSuccess { get; set; } // Criterio: Confirmación exitosa/fallida
        public string ErrorMessage { get; set; } // Criterio: Manejo de errores
    }
}