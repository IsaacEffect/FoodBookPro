using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FoodBookPro.Test
{

    public class ReservaRepositoryTests
    {
        // =====================================================================
        // Método helper
        // =====================================================================
        private Reserva CrearReserva(
            Guid? clienteId = null,
            string nombreCliente = "Juan Pérez",
            DateTime? fechaHora = null,
            int numeroDPersonas = 2,
            bool confirmacionInmediata = false)
        {
            return Reserva.Crear(
                clienteId ?? Guid.NewGuid(),
                nombreCliente,
                "juan@email.com",
                "8091234567",
                fechaHora ?? DateTime.UtcNow.AddDays(1),
                numeroDPersonas,
                confirmacionInmediata);
        }

        // =====================================================================
        // CrearAsync
        // =====================================================================

        [Fact]
        public async Task CrearAsync_CuandoReservaEsValida_DebeRetornarReservaCreada()
        {
            // Arrange
            var repository = new ReservaRepository();
            var reserva = CrearReserva(confirmacionInmediata: false);

            // Act
            var resultado = await repository.CrearAsync(reserva);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEqual(Guid.Empty, resultado.Id);
            Assert.Equal(EstadoReserva.Pendiente, resultado.Estado);
        }

        // =====================================================================
        // ObtenerPorIdAsync
        // =====================================================================

        [Fact]
        public async Task ObtenerPorIdAsync_CuandoIdExiste_DebeRetornarReserva()
        {
            // Arrange
            var repository = new ReservaRepository();
            var reserva = CrearReserva();
            await repository.CrearAsync(reserva);

            // Act
            var resultado = await repository.ObtenerPorIdAsync(reserva.Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(reserva.Id, resultado.Id);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_CuandoIdNoExiste_DebeRetornarNull()
        {
            // Arrange
            var repository = new ReservaRepository();

            // Act
            var resultado = await repository.ObtenerPorIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(resultado);
        }

        // =====================================================================
        // ModificarAsync
        // =====================================================================

        [Fact]
        public async Task ModificarAsync_CuandoIdExiste_DebeReflejarLosCambios()
        {
            // Arrange
            var repository = new ReservaRepository();
            var reserva = CrearReserva(numeroDPersonas: 2);
            await repository.CrearAsync(reserva);
            reserva.Modificar(DateTime.UtcNow.AddDays(5), 6, "Mesa cerca de la ventana");

            // Act
            var resultado = await repository.ModificarAsync(reserva);

            // Assert
            Assert.Equal(6, resultado.NumeroDPersonas);
            Assert.Equal("Mesa cerca de la ventana", resultado.ComentariosEspeciales);
        }

        [Fact]
        public async Task ModificarAsync_CuandoIdNoExiste_DebeLanzarKeyNotFoundException()
        {
            // Arrange
            var repository = new ReservaRepository();
            var inexistente = CrearReserva();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.ModificarAsync(inexistente));
        }

        // =====================================================================
        // EliminarAsync
        // =====================================================================

        [Fact]
        public async Task EliminarAsync_CuandoIdExiste_DebeEliminarLaReserva()
        {
            // Arrange
            var repository = new ReservaRepository();
            var reserva = CrearReserva();
            await repository.CrearAsync(reserva);

            // Act
            await repository.EliminarAsync(reserva.Id);
            var resultado = await repository.ObtenerPorIdAsync(reserva.Id);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task EliminarAsync_CuandoIdNoExiste_DebeLanzarKeyNotFoundException()
        {
            // Arrange
            var repository = new ReservaRepository();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.EliminarAsync(Guid.NewGuid()));
        }

        // =====================================================================
        // ConfirmarAsync / CancelarAsync
        // =====================================================================

        [Fact]
        public async Task ConfirmarAsync_CuandoIdExiste_DebeCambiarEstadoAConfirmada()
        {
            // Arrange
            var repository = new ReservaRepository();
            var reserva = CrearReserva(confirmacionInmediata: false);
            await repository.CrearAsync(reserva);

            // Act
            var resultado = await repository.ConfirmarAsync(reserva.Id);

            // Assert
            Assert.Equal(EstadoReserva.Confirmada, resultado.Estado);
        }

        [Fact]
        public async Task CancelarAsync_CuandoIdExiste_DebeCambiarEstadoACancelada()
        {
            // Arrange
            var repository = new ReservaRepository();
            var reserva = CrearReserva(confirmacionInmediata: false);
            await repository.CrearAsync(reserva);

            // Act
            var resultado = await repository.CancelarAsync(reserva.Id);

            // Assert
            Assert.Equal(EstadoReserva.Cancelada, resultado.Estado);
        }

        // =====================================================================
        // VerificarDisponibilidadAsync
        // =====================================================================

        [Fact]
        public async Task VerificarDisponibilidadAsync_CuandoCapacidadSuperada_DebeRetornarFalse()
        {
            // Arrange
            var repository = new ReservaRepository();
            var fechaHora = DateTime.UtcNow.AddDays(1);
            await repository.CrearAsync(CrearReserva(
                fechaHora: fechaHora,
                numeroDPersonas: 48,
                confirmacionInmediata: true));

            // Act
            var resultado = await repository.VerificarDisponibilidadAsync(fechaHora, 5);

            // Assert
            Assert.False(resultado);
        }
    }

}