using FoodBookPro.Data.Entities;

namespace FoodBookPro.Data.Interfaces
{
    public interface IPropietarioRepository
    {
        Task AddAsync(Propietario propietario);

        Task<Propietario?> GetByEmailAsync(string email);

        Task<bool> ExistsByEmailAsync(string email);
    }
}