using FoodBookPro.Data.Entities;
using FoodBookPro.Data.Interfaces;
using FoodBookPro.Data.Services;
using Moq;

namespace FoodBookPro.Test
{
    public class ClienteServiceTest
    {
        private readonly Mock<IClienteRepository> _repoMock;
        private readonly ClienteService _service;

        public ClienteServiceTest()
        {
            _repoMock = new Mock<IClienteRepository>();
            _service = new ClienteService(_repoMock.Object);
        }

        [Fact]
        public async Task Registro_Exitoso()
        {
            // Arrange
            _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync(false);

            // Act
            var result = await _service.RegistrarAsync(
                "Alberto",
                "A.toribio24@hotmail.com",
                "mr23p=lA",
                "8091230502"
            );

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task Registro_Falla_EmailYaExiste()
        {
            // Arrange
            _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "Alberto",
                    "A.toribio24@hotmail.com",
                    "mr23p=lA",
                    "8091230502"
                )
            );

            _repoMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task Registro_Falla_EmailInvalido()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "Alberto",
                    "atoribio24hotmail.com",
                    "mr23p=lA",
                    "8091230502"
                )
            );
        }

        [Fact]
        public async Task Registro_Falla_PasswordMuyCorta()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "Alberto",
                    "A.toribio24@hotmail.com",
                    "123",
                    "8091230502"
                )
            );
        }

        [Fact]
        public async Task Registro_Falla_NombreVacio()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "",
                    "A.toribio24@hotmail.com",
                    "mr23p=lA",
                    "8091230502"
                )
            );
        }
    }
}