using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FoodBookPro.Test
{

    public class DisponibilidadRepositoryTests
    {
        // =====================================================================
        // Método helper
        // =====================================================================
        private DisponibilidadRestaurante CrearDisponibilidad(
            DayOfWeek dia = DayOfWeek.Monday,
            TimeSpan? apertura = null,
            TimeSpan? cierre = null,
            int capacidad = 50,
            int maxReservas = 10,
            bool confirmacionInmediata = true)
        {
            return DisponibilidadRestaurante.Crear(
                dia,
                apertura ?? TimeSpan.FromHours(9),
                cierre ?? TimeSpan.FromHours(22),
                capacidad,
                maxReservas,
                confirmacionInmediata);
        }

        // =====================================================================
        // ObtenerPorDiaAsync
        // =====================================================================

        [Fact]
        public async Task ObtenerPorDiaAsync_CuandoDiaExiste_DebeRetornarDisponibilidad()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            await repository.CrearAsync(CrearDisponibilidad(dia: DayOfWeek.Monday));

            // Act
            var resultado = await repository.ObtenerPorDiaAsync(DayOfWeek.Monday);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(DayOfWeek.Monday, resultado.DiaSemana);
        }

        [Fact]
        public async Task ObtenerPorDiaAsync_CuandoDiaNoExiste_DebeRetornarNull()
        {
            // Arrange
            var repository = new DisponibilidadRepository();

            // Act
            var resultado = await repository.ObtenerPorDiaAsync(DayOfWeek.Friday);

            // Assert
            Assert.Null(resultado);
        }

        // =====================================================================
        // ObtenerTodasAsync
        // =====================================================================

        [Fact]
        public async Task ObtenerTodasAsync_CuandoNoHayRegistros_DebeRetornarColeccionVacia()
        {
            // Arrange
            var repository = new DisponibilidadRepository();

            // Act
            var resultado = await repository.ObtenerTodasAsync();

            // Assert
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObtenerTodasAsync_CuandoHayRegistros_DebeRetornarTodos()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            await repository.CrearAsync(CrearDisponibilidad(dia: DayOfWeek.Monday));
            await repository.CrearAsync(CrearDisponibilidad(dia: DayOfWeek.Tuesday));
            await repository.CrearAsync(CrearDisponibilidad(dia: DayOfWeek.Wednesday));

            // Act
            var resultado = await repository.ObtenerTodasAsync();

            // Assert
            Assert.Equal(3, resultado.Count());
        }

        // =====================================================================
        // CrearAsync
        // =====================================================================

        [Fact]
        public async Task CrearAsync_CuandoDiaNoExiste_DebeRetornarDisponibilidadCreada()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            var disponibilidad = CrearDisponibilidad(
                dia: DayOfWeek.Saturday,
                apertura: TimeSpan.FromHours(10),
                cierre: TimeSpan.FromHours(23),
                capacidad: 100,
                maxReservas: 20);

            // Act
            var resultado = await repository.CrearAsync(disponibilidad);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(DayOfWeek.Saturday, resultado.DiaSemana);
            Assert.Equal(100, resultado.CapacidadMaximaPersonas);
            Assert.True(resultado.EstaDisponible);
        }

        [Fact]
        public async Task CrearAsync_CuandoDiaYaExiste_DebeLanzarInvalidOperationException()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            await repository.CrearAsync(CrearDisponibilidad(dia: DayOfWeek.Monday));
            var duplicada = CrearDisponibilidad(dia: DayOfWeek.Monday);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => repository.CrearAsync(duplicada));
        }

        [Fact]
        public async Task CrearAsync_CuandoDiaYaExiste_MensajeDeExcepcionDebeContenerElDia()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            await repository.CrearAsync(CrearDisponibilidad(dia: DayOfWeek.Monday));
            var duplicada = CrearDisponibilidad(dia: DayOfWeek.Monday);

            // Act
            var excepcion = await Assert.ThrowsAsync<InvalidOperationException>(
                () => repository.CrearAsync(duplicada));

            // Assert
            Assert.Contains(DayOfWeek.Monday.ToString(), excepcion.Message);
        }

        // =====================================================================
        // ActualizarAsync
        // =====================================================================

        [Fact]
        public async Task ActualizarAsync_CuandoIdExiste_DebeActualizarYRetornarDisponibilidad()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            var disponibilidad = CrearDisponibilidad(dia: DayOfWeek.Thursday);
            await repository.CrearAsync(disponibilidad);
            disponibilidad.Actualizar(
                TimeSpan.FromHours(8), TimeSpan.FromHours(20),
                30, 5, false, false);

            // Act
            var resultado = await repository.ActualizarAsync(disponibilidad);

            // Assert
            Assert.Equal(TimeSpan.FromHours(8), resultado.HoraApertura);
            Assert.Equal(30, resultado.CapacidadMaximaPersonas);
            Assert.False(resultado.EstaDisponible);
        }

        [Fact]
        public async Task ActualizarAsync_CuandoIdNoExiste_DebeLanzarKeyNotFoundException()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            var inexistente = CrearDisponibilidad(dia: DayOfWeek.Wednesday);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.ActualizarAsync(inexistente));
        }

        [Fact]
        public async Task ActualizarAsync_CuandoIdNoExiste_MensajeDeExcepcionDebeContenerElId()
        {
            // Arrange
            var repository = new DisponibilidadRepository();
            var disponibilidad = CrearDisponibilidad(dia: DayOfWeek.Wednesday);

            // Act
            var excepcion = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => repository.ActualizarAsync(disponibilidad));

            // Assert
            Assert.Contains(disponibilidad.Id.ToString(), excepcion.Message);
        }
    }

}