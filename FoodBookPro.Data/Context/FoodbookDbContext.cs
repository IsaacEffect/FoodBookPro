using Microsoft.EntityFrameworkCore;
using FoodBookPro.Data.Entities; // Para que reconozca tus clases

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
        // Aquí registramos nuestras "tablas"
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usamos una base de datos en memoria aun no he configurado el sql
            optionsBuilder.UseInMemoryDatabase("FoodbookDb");
        }
    }
}