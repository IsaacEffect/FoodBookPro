using FoodBookPro.Data.Base;
using FoodBookPro.Data.Entities;
using FoodBookPro.Data.Interfaces;
using FoodBookPro.Data.Services;
using Moq;

namespace FoodBookPro.Test
{
    public class PropietarioServiceTest
    {
        private readonly Mock<IPropietarioRepository> _repoMock;
        private readonly PropietarioService _service;

        public PropietarioServiceTest()
        {
            _repoMock = new Mock<IPropietarioRepository>();
            _service = new PropietarioService(_repoMock.Object);
        }

        [Fact]
        public async Task Registro_Propietario_Exitoso()
        {
            _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync(false);

            var result = await _service.RegistrarAsync(
                "Richard Matos",
                "richard.matos24@gmail.com",
                "Rm2024$",
                "8294567821",
                "Sabor Criollo RD"
            );

            Assert.True(result);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Propietario>()), Times.Once);
        }

        [Fact]
        public async Task Registro_Falla_EmailDuplicado()
        {
            _repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync(true);

            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "Richard Matos",
                    "richard.matos24@gmail.com",
                    "Rm2024$",
                    "8294567821",
                    "Sabor Criollo RD"
                )
            );
        }

        [Fact]
        public async Task Registro_Falla_EmailInvalido()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "Richard Matos",
                    "richard.matos24gmail.com",
                    "Rm2024$",
                    "8294567821",
                    "Sabor Criollo RD"
                )
            );
        }

        [Fact]
        public async Task Registro_Falla_PasswordCorta()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "Richard Matos",
                    "richard.matos24@gmail.com",
                    "123",
                    "8294567821",
                    "Sabor Criollo RD"
                )
            );
        }

        [Fact]
        public async Task Registro_Falla_NombreVacio()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.RegistrarAsync(
                    "",
                    "richard.matos24@gmail.com",
                    "Rm2024$",
                    "8294567821",
                    "Sabor Criollo RD"
                )
            );
        }

        [Fact]
        public async Task Aprobar_Propietario_CambiaEstado()
        {
            var propietario = new Propietario
            {
                Id = Guid.NewGuid(),
                Nombre = "Richard Matos",
                Email = "richard.matos24@gmail.com",
                Estado = EstadoPropietario.Pendiente
            };

            _repoMock.Setup(r => r.GetByIdAsync(propietario.Id))
                     .ReturnsAsync(propietario);

            await _service.AprobarPropietarioAsync(propietario.Id);

            Assert.Equal(EstadoPropietario.Aprobado, propietario.Estado);
        }

        [Fact]
        public async Task Rechazar_Propietario_CambiaEstado()
        {
            var propietario = new Propietario
            {
                Id = Guid.NewGuid(),
                Nombre = "Richard Matos",
                Email = "richard.matos24@gmail.com",
                Estado = EstadoPropietario.Pendiente
            };

            _repoMock.Setup(r => r.GetByIdAsync(propietario.Id))
                     .ReturnsAsync(propietario);

            await _service.RechazarPropietarioAsync(propietario.Id);

            Assert.Equal(EstadoPropietario.Rechazado, propietario.Estado);
        }

        [Fact]
        public async Task Login_Propietario_Aprobado_PuedeEntrar()
        {
            var propietario = new Propietario
            {
                Id = Guid.NewGuid(),
                Nombre = "Richard Matos",
                Email = "richard.matos24@gmail.com",
                PasswordHash = "Rm2024$",
                Estado = EstadoPropietario.Aprobado
            };

            _repoMock.Setup(r => r.GetByEmailAsync(propietario.Email))
                     .ReturnsAsync(propietario);

            var result = await _service.AutenticarAsync(
                "richard.matos24@gmail.com",
                "Rm2024$"
            );

            Assert.NotNull(result);
            Assert.Equal("Richard Matos", result.Nombre);
        }

        [Fact]
        public async Task Login_Propietario_Pendiente_NoPuedeEntrar()
        {
            var propietario = new Propietario
            {
                Id = Guid.NewGuid(),
                Nombre = "Richard Matos",
                Email = "richard.matos24@gmail.com",
                PasswordHash = "Rm2024$",
                Estado = EstadoPropietario.Pendiente
            };

            _repoMock.Setup(r => r.GetByEmailAsync(propietario.Email))
                     .ReturnsAsync(propietario);

            await Assert.ThrowsAsync<Exception>(() =>
                _service.AutenticarAsync(
                    "richard.matos24@gmail.com",
                    "Rm2024$"
                )
            );
        }
    }
}