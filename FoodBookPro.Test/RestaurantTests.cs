using Microsoft.EntityFrameworkCore;
using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FoodBookPro.Test
{
    public class RestaurantTests
    {
        private FoodbookDbContext GetDbContext()
        {
            // CONFIGURACIÓN CLAVE: Usamos una base de datos en memoria 
            // con un nombre único para que cada test sea independiente.
            var options = new DbContextOptionsBuilder<FoodbookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new FoodbookDbContext(options);

            // Llenamos la base de datos de "juguete" con tu SeedData
            context.SeedData();
            return context;
        }

        [Fact]
        public void Index_ReturnsAllRestaurants_WhenNoFiltersApplied()
        {
            // 1. Arrange (Preparar)
            using var context = GetDbContext();
            var controller = new RestaurantController(context);

            // 2. Act (Actuar) - Llamamos al Index sin filtros
            var result = controller.Index(null, null, null, null, null, null);

            // 3. Assert (Verificar)
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Restaurant>>(viewResult.ViewData.Model);

            // Verificamos que trajo los 3 restaurantes que definiste en tu SeedData
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public void XAV_35_Filter_By_Cuisine_Works()
        {
            // Este test prueba específicamente tu tarea de Jira
            using var context = GetDbContext();
            var controller = new RestaurantController(context);

            // Filtramos por "Italiana"
            var result = controller.Index("Pizza", "Italiana", null, null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Restaurant>>(viewResult.ViewData.Model);

            // Solo debería haber 1: Pizza Palace
            Assert.Single(model);
            Assert.Equal("Pizza Palace", model.First().Name);
        }
    }
}