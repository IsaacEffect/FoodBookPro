using Microsoft.EntityFrameworkCore;
using FoodBookPro.Data.Entities;

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
        public FoodbookDbContext() { }

        public FoodbookDbContext(DbContextOptions<FoodbookDbContext> options)
            : base(options)
        {
        }

        // Aquí registramos TODAS las tablas del equipo (XAV-53 + XAV-26)
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

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
                entity.HasIndex(c => c.Email).IsUnique();
            });

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderStatusHistory>()
                .HasOne<Order>()
                .WithMany(o => o.HistorialEstados)
                .HasForeignKey(h => h.OrderId);
        }

        /// <summary>
        /// Carga datos de ejemplo para desarrollo (XAV-176)
        /// </summary>
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
            var items = new List<OrderItem>
            {
                new OrderItem { Id = 1, OrderId = 1, ProductoNombre = "Pizza Margarita", Cantidad = 2, Precio = 12.75m },
                new OrderItem { Id = 2, OrderId = 2, ProductoNombre = "Hamburguesa", Cantidad = 2, Precio = 9.00m },
                new OrderItem { Id = 3, OrderId = 3, ProductoNombre = "Roll Sushi", Cantidad = 3, Precio = 14.25m },
                new OrderItem { Id = 4, OrderId = 4, ProductoNombre = "Pizza Napolitana", Cantidad = 1, Precio = 15.00m },
                new OrderItem { Id = 5, OrderId = 5, ProductoNombre = "Combo Burger", Cantidad = 1, Precio = 12.00m }
            };
            OrderItems.AddRange(items);
            SaveChanges();
        }
    }
}