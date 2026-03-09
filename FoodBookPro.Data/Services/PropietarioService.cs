using FoodBookPro.Data.Base;
using FoodBookPro.Data.Entities;
using FoodBookPro.Data.Interfaces;

namespace FoodBookPro.Data.Services
{
    public class PropietarioService
    {
        private readonly IPropietarioRepository _repository;

        public PropietarioService(IPropietarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> RegistrarAsync(
            string nombre,
            string email,
            string password,
            string telefono,
            string restaurante)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre es obligatorio");

            if (!email.Contains("@"))
                throw new Exception("Email inválido");

            if (password.Length < 6)
                throw new Exception("Contraseña muy corta");

            if (await _repository.ExistsByEmailAsync(email))
                throw new Exception("El email ya está registrado");

            var propietario = new Propietario
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Email = email,
                PasswordHash = password,
                Telefono = telefono,
                RestauranteNombre = restaurante,
                Estado = EstadoPropietario.Pendiente
            };

            await _repository.AddAsync(propietario);

            return true;
        }
    }
}