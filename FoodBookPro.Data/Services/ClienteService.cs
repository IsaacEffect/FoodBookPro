using FoodBookPro.Data.Entities;
using FoodBookPro.Data.Interfaces;

namespace FoodBookPro.Data.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _repository;

        public ClienteService(IClienteRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> RegistrarAsync(string nombre, string email, string password, string telefono)
        {
            // Validaciones de negocio

            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("El email es obligatorio");

            if (!email.Contains("@"))
                throw new Exception("Email inválido");

            if (password.Length < 6)
                throw new Exception("La contraseña debe tener mínimo 6 caracteres");

            if (await _repository.ExistsByEmailAsync(email))
                throw new Exception("El email ya existe");

            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Email = email,
                PasswordHash = password,
                Telefono = telefono
            };

            await _repository.AddAsync(cliente);

            return true;
        }

        // LOGIN
        public async Task<Cliente> AutenticarAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("El email es obligatorio");

            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("La contraseña es obligatoria");

            var cliente = await _repository.GetByEmailAsync(email) ?? throw new Exception("Usuario no encontrado");
            if (cliente.PasswordHash != password)
                throw new Exception("Credenciales inválidas");

            return cliente;
        }
    }
}
