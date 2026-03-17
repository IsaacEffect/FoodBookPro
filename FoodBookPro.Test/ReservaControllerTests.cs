using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodBookPro.Test
{
    public class ReservaControllerTests
    {
        private FoodbookDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<FoodbookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new FoodbookDbContext(options);
        }

        [Fact]
        public void XAV53_Disponibilidad_VerificarHorario_Funciona()
        {
            // Arrange
            var apertura = new TimeSpan(18, 0, 0);
            var cierre = new TimeSpan(23, 0, 0);

            // CORRECCIÓN: Si agregaste 'restauranteId' como primer parámetro (int), pon un 1.
            // Si tu 'Crear' usa Guids, usa Guid.NewGuid().
            // --- CORRECCIÓN AQUÍ ---
            var disp = DisponibilidadRestaurante.Crear(
                1,                  // 1. Agrega este '1' (es el ID del restaurante)
                DayOfWeek.Friday,   // 2. Ahora sí, el día de la semana
                apertura,           // 3. Hora de apertura
                cierre,             // 4. Hora de cierre
                50,                 // 5. Capacidad máxima
                true                // 6. Confirmación inmediata
            
            );

            // Act
            var esValido = disp.EstaEnHorario(new TimeSpan(20, 0, 0));

            // Assert
            Assert.True(esValido);
        }

        [Fact]
        public void XAV53_Reserva_DebeCrearseConEstadoPendiente()
        {
            // Arrange: Usamos Guids y tus nombres reales: 'fechaHora' y 'numeroDPersonas'
            var reserva = Reserva.Crear(
                clienteId: Guid.NewGuid(),
                nombreCliente: "Juan Perez",
                emailCliente: "juan@mail.com",
                telefonoCliente: "8295551234",
                fechaHora: DateTime.Now.AddDays(1),
                numeroDPersonas: 4,
                confirmacionInmediata: false
            );

            // Assert
            Assert.Equal(EstadoReserva.Pendiente, reserva.Estado);
            Assert.NotNull(reserva.CodigoReserva);
        }
    }
}