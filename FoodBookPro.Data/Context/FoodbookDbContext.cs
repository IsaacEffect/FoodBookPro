using FoodBookPro.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public FoodbookDbContext() { }

        public FoodbookDbContext(DbContextOptions<FoodbookDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseInMemoryDatabase("FoodbookDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);
        }

        /// <summary>
        /// Carga datos de ejemplo para desarrollo
        /// </summary>
        public void SeedData()
        {
            if (Orders.Any()) return;

            var orders = new List<Order>
            {
                new Order { Id = 1, Fecha = DateTime.Now.AddDays(-3), Estado = EstadoOrden.Completada, RestauranteNombre = "Pizza Palace", Total = 25.50m },
                new Order { Id = 2, Fecha = DateTime.Now.AddDays(-1), Estado = EstadoOrden.Preparando, RestauranteNombre = "Burger Spot", Total = 18.00m },
                new Order { Id = 3, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "Sushi Bar", Total = 42.75m },
                new Order { Id = 4, Fecha = DateTime.Now.AddDays(-5), Estado = EstadoOrden.Completada, RestauranteNombre = "Pizza Palace", Total = 15.00m },
                new Order { Id = 5, Fecha = DateTime.Now.AddDays(-2), Estado = EstadoOrden.Cancelada, RestauranteNombre = "Burger Spot", Total = 12.00m }
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
