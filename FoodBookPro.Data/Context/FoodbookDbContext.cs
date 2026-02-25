using Microsoft.EntityFrameworkCore;
using FoodBookPro.Data.Entities;

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
        public FoodbookDbContext(DbContextOptions<FoodbookDbContext> options)
        : base(options)
        {
        }
        // Aquí registramos nuestras "tablas"
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nombre)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasIndex(c => c.Email)
                      .IsUnique();
            });
        }
    }
}