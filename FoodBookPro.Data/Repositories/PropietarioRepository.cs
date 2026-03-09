using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Data.Repositories
{
    public class PropietarioRepository : IPropietarioRepository
    {
        private readonly FoodbookDbContext _context;

        public PropietarioRepository(FoodbookDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Propietario propietario)
        {
            await _context.Propietarios.AddAsync(propietario);
            await _context.SaveChangesAsync();
        }

        public async Task<Propietario?> GetByEmailAsync(string email)
        {
            return await _context.Propietarios
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Propietarios
                .AnyAsync(p => p.Email == email);
        }
    }
}