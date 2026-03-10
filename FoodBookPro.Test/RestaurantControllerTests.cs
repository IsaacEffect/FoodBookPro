using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodBookPro.Test
{
    public class RestaurantControllerTests
    {
        private FoodbookDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<FoodbookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new FoodbookDbContext(options);

            // Seed de datos para las pruebas
            context.Restaurants.AddRange(new List<Restaurant>
            {
                new Restaurant { Id = 1, Name = "Pizza Planeta", City = "Santo Domingo", CuisineType = "Italiana", PriceRange = "$", Rating = 4.5, Distance = 0.5 },
                new Restaurant { Id = 2, Name = "Sushi Master", City = "Santo Domingo", CuisineType = "Japonesa", PriceRange = "$$$", Rating = 4.8, Distance = 5.0 },
                new Restaurant { Id = 3, Name = "Burger Galaxy", City = "Santiago", CuisineType = "Hamburguesas", PriceRange = "$$", Rating = 4.2, Distance = 2.0 }
            });
            context.SaveChanges();
            return context;
        }

        // --- PRUEBA 1 (XAV-26): BÚSQUEDA POR NOMBRE ---
        [Fact]
        public void XAV26_BusquedaPorNombre_RetornaResultadosCorrectos()
        {
            var context = GetDbContext();
            var controller = new RestaurantController(context);
            var result = controller.Index("Pizza", null, null, null, null, null) as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Restaurant>>(result.Model);
            Assert.Single(model);
        }

        // --- PRUEBA 2 (XAV-34): FILTRO POR ESPECIALIDAD ---
        [Fact]
        public void XAV34_FiltroPorCuisine_Funciona()
        {
            var context = GetDbContext();
            var controller = new RestaurantController(context);
            var result = controller.Index(null, "Japonesa", null, null, null, null) as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Restaurant>>(result.Model);
            Assert.All(model, r => Assert.Equal("Japonesa", r.CuisineType));
        }

        // --- PRUEBA 3 (XAV-34): FILTRO POR PRECIO ---
        [Fact]
        public void XAV34_FiltroPorPrecioMaximo_Funciona()
        {
            var context = GetDbContext();
            var controller = new RestaurantController(context);
            // Filtramos por precio económico ($)
            var result = controller.Index(null, null, "$", null, null, null) as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Restaurant>>(result.Model);
            Assert.Contains(model, r => r.PriceRange == "$");
        }

        // --- PRUEBA 4 (XAV-34): FILTRO POR DISTANCIA ---
        [Fact]
        public void XAV34_FiltroPorDistanciaMaxima_Funciona()
        {
            var context = GetDbContext();
            var controller = new RestaurantController(context);
            // Máximo 1km (Solo Pizza Planeta está a 0.5km)
            var result = controller.Index(null, null, null, null, 1.0, null) as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Restaurant>>(result.Model);
            Assert.Single(model);
        }

        // --- PRUEBA 5 (XAV-34): FILTRO POR CALIFICACIÓN MÍNIMA ---
        [Fact]
        public void XAV34_FiltroPorRatingMinimo_Funciona()
        {
            var context = GetDbContext();
            var controller = new RestaurantController(context);
            // Buscamos restaurantes con más de 4.7 (Solo Sushi Master tiene 4.8)
            var result = controller.Index(null, null, null, 4.7, null, null) as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Restaurant>>(result.Model);
            Assert.Single(model);
            Assert.Equal("Sushi Master", model.First().Name);
        }
    }
}