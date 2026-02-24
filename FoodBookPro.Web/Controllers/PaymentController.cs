using Microsoft.AspNetCore.Mvc;
using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using System.Linq;

namespace FoodBookPro.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly FoodbookDbContext _context;

        public PaymentController()
        {
            // Inicializamos el contexto (Cimiento de datos)
            _context = new FoodbookDbContext();

            // Creamos una orden de prueba si no existe ninguna
            if (!_context.Orders.Any())
            {
                _context.Orders.Add(new Order { Id = 1, CustomerName = "Cliente X-Men", Total = 50.00m, Status = "Pendiente" });
                _context.SaveChanges();
            }
        }

        // Criterio: Resumen de orden antes de pagar
        public IActionResult Checkout(int id = 1)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // Criterio: Procesamiento, Propina y Manejo de errores
        [HttpPost]
        public IActionResult Process(int orderId, decimal tipAmount, string method)
        {
            var order = _context.Orders.Find(orderId);

            // Manejo de errores técnico
            if (order == null || tipAmount < 0)
            {
                return RedirectToAction("Checkout", new { id = orderId });
            }

            var payment = new Payment
            {
                OrderId = orderId,
                Subtotal = order.Total,
                TipAmount = tipAmount,
                PaymentMethod = method,
                TransactionDate = System.DateTime.Now,
                IsSuccess = true
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Simulación de Criterio: Recibo digital enviado por email
            System.Diagnostics.Debug.WriteLine($"Recibo enviado al cliente. Total: {payment.TotalAmount}");

            // Redirección a Criterio: Confirmación de pago exitoso
            return View("Success", payment);
        }
    }
}