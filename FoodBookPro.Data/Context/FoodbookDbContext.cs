using FoodBookPro.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
using Microsoft.EntityFrameworkCore;
using FoodBookPro.Data.Entities;

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
        public FoodbookDbContext() { }

        public FoodbookDbContext(DbContextOptions<FoodbookDbContext> options) : base(options) { }

        // Aquí registramos TODAS las tablas del equipo (XAV-53 + XAV-26)
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; } // Tu buscador

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseInMemoryDatabase("FoodbookDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Clientes (de develop)
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(c => c.Email).IsUnique();
            });

            // Configuración de Items de Orden (de develop)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            // Criterio XAV-26: Seed Data con tus 12 restaurantes
            modelBuilder.Entity<Restaurant>().HasData(
                new Restaurant { Id = 1, Name = "Pizza Planeta", City = "Santo Domingo", CuisineType = "Italiana", Rating = 4.5, ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400" },
                new Restaurant { Id = 2, Name = "Burger Galaxy", City = "Santiago", CuisineType = "Hamburguesas", Rating = 4.2, ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=400" },
                new Restaurant { Id = 3, Name = "Sushi Master", City = "Santo Domingo", CuisineType = "Japonesa", Rating = 4.8, ImageUrl = "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?w=400" },
                new Restaurant { Id = 4, Name = "Taco Loco", City = "La Romana", CuisineType = "Mexicana", Rating = 4.0, ImageUrl = "https://images.unsplash.com/photo-1565299585323-38d6b0865b47?w=400" },
                new Restaurant { Id = 5, Name = "El Rincón del Pollo", City = "Santo Domingo", CuisineType = "Criolla", Rating = 4.3, ImageUrl = "https://images.unsplash.com/photo-1562967914-608f82629710?w=400" },
                new Restaurant { Id = 6, Name = "Pasta & Vino", City = "Punta Cana", CuisineType = "Italiana", Rating = 4.7, ImageUrl = "https://images.unsplash.com/photo-1473093226795-af9932fe5856?w=400" },
                new Restaurant { Id = 7, Name = "Veggie Delight", City = "Santo Domingo", CuisineType = "Vegetariana", Rating = 4.6, ImageUrl = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400" },
                new Restaurant { Id = 8, Name = "Steak House 85", City = "Santiago", CuisineType = "Carnes", Rating = 4.4, ImageUrl = "https://images.unsplash.com/photo-1544025162-d76694265947?w=400" },
                new Restaurant { Id = 9, Name = "Wok & Roll", City = "Santo Domingo", CuisineType = "Asiática", Rating = 4.1, ImageUrl = "https://images.unsplash.com/photo-1512058560566-42724afbc2db?w=400" },
                new Restaurant { Id = 10, Name = "La Dulce Parada", City = "Puerto Plata", CuisineType = "Postres", Rating = 4.9, ImageUrl = "https://images.unsplash.com/photo-1551024601-bec78aea704b?w=400" },
                new Restaurant { Id = 11, Name = "Mariscos del Caribe", City = "Boca Chica", CuisineType = "Mariscos", Rating = 4.2, ImageUrl = "https://images.unsplash.com/photo-1551489186-cf8726f514f8?w=400" },
                new Restaurant { Id = 12, Name = "Café Central", City = "Santo Domingo", CuisineType = "Cafetería", Rating = 4.5, ImageUrl = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=400" }
            );
        }

        // Método para cargar datos de órdenes (de develop)
        public void SeedData()
        {
            if (Orders.Any()) return;
            var now = DateTime.Now;
            var orders = new List<Order>
            {
                new Order { Id = 1, Fecha = now.AddDays(-3), Estado = EstadoOrden.Completada, RestauranteNombre = "Pizza Palace", Total = 25.50m, HoraRetiro = now.AddDays(-3).AddMinutes(30) },
                new Order { Id = 2, Fecha = now.AddDays(-1), Estado = EstadoOrden.Preparando, RestauranteNombre = "Burger Spot", Total = 18.00m, HoraRetiro = now.AddDays(-1).AddMinutes(45) },
                new Order { Id = 3, Fecha = now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "Sushi Bar", Total = 42.75m, HoraRetiro = now.AddMinutes(60) }
            };
            Orders.AddRange(orders);
            SaveChanges();
        }
    }
}
        }
    }
}
