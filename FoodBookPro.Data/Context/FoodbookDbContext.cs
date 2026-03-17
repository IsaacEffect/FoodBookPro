using Microsoft.EntityFrameworkCore;
using FoodBookPro.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
        public FoodbookDbContext() { }

        public FoodbookDbContext(DbContextOptions<FoodbookDbContext> options)
            : base(options)
        {
        }

        // Aquí registramos TODAS las tablas del equipo (XAV-53 + XAV-26 + reservas)
        // --- Tablas registradas ---
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<DisponibilidadRestaurante> Disponibilidades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FoodBookProDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuración GLOBAL para Decimales (Precios, Totales, etc.)
            // Esto quita todos los warnings de "No store type specified"
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            // 2. Configuración de Coordenadas (Alta precisión para GPS)
            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(r => r.Latitude).HasColumnType("decimal(18,10)");
                entity.Property(r => r.Longitude).HasColumnType("decimal(18,10)");
            });

            // 3. Configuración de Clientes
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(c => c.Email).IsUnique();
            });

            // 4. Relación Order -> Items
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. Relación Order -> Historial (Corrige el error de OrderId1)
            modelBuilder.Entity<OrderStatusHistory>()
                .HasOne(h => h.Order) // Mapeo explícito a la propiedad de navegación
                .WithMany(o => o.HistorialEstados)
                .HasForeignKey(h => h.OrderId);
        }

        public void SeedData()
        {
            if (Orders.Any() || Restaurants.Any()) return;

            var now = DateTime.Now;

            var sampleRestaurants = new List<Restaurant>
            {
                new Restaurant { Name = "Pizza Palace", Latitude = 18.4861, Longitude = -69.9312, CuisineType = "Italiana", Rating = 4.5 },
                new Restaurant { Name = "Burger Spot", Latitude = 18.4830, Longitude = -69.9400, CuisineType = "Americana", Rating = 4.0 },
                new Restaurant { Name = "Sushi Bar", Latitude = 18.4750, Longitude = -69.9250, CuisineType = "Japonesa", Rating = 4.8 }
            };
            Restaurants.AddRange(sampleRestaurants);

            var orders = new List<Order>
            {
                new Order { Fecha = now.AddDays(-3), Estado = EstadoOrden.Completada, RestauranteNombre = "Pizza Palace", Total = 25.50m, HoraRetiro = now.AddDays(-3).AddMinutes(30) },
                new Order { Fecha = now.AddDays(-1), Estado = EstadoOrden.Preparando, RestauranteNombre = "Burger Spot", Total = 18.00m, HoraRetiro = now.AddDays(-1).AddMinutes(45) },
                new Order { Fecha = now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "Sushi Bar", Total = 42.75m, HoraRetiro = now.AddMinutes(60) }
            };
            Orders.AddRange(orders);
            SaveChanges();

            var items = new List<OrderItem>
            {
                new OrderItem { OrderId = orders[0].Id, ProductoNombre = "Pizza Margarita", Cantidad = 2, Precio = 12.75m },
                new OrderItem { OrderId = orders[1].Id, ProductoNombre = "Hamburguesa", Cantidad = 2, Precio = 9.00m },
                new OrderItem { OrderId = orders[2].Id, ProductoNombre = "Roll Sushi", Cantidad = 3, Precio = 14.25m }
            };
            OrderItems.AddRange(items);
            SaveChanges();
        }
    }
}