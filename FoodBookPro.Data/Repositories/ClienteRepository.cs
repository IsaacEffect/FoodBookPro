using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly FoodbookDbContext _context;

        public ClienteRepository(FoodbookDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            return await _context.Clientes
                   .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Clientes
                   .AnyAsync(c => c.Email == email);
        }

        public async Task AddAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }
    }
}