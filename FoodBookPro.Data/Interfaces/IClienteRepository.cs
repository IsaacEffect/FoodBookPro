using FoodBookPro.Data.Entities;

namespace FoodBookPro.Data.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> GetByEmailAsync(string email);
        Task AddAsync(Cliente cliente);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
