using Microsoft.AspNetCore.Mvc;
using FoodBookPro.Data.Context; // Para reconocer el DbContext
using FoodBookPro.Data.Entities; // Para reconocer la entidad Restaurant
using System.Linq;

namespace FoodBookPro.Web.Controllers
{
    // 1. La clase debe heredar de 'Controller' para que ViewBag y View existan
    public class RestaurantController : Controller
    {
        private readonly FoodbookDbContext _context;

        // 2. Inyectamos el contexto para arreglar el error de '_context'
        public RestaurantController(FoodbookDbContext context)
        {
            _context = context;
        }

        // 3. Método Index con todos los filtros de la XAV-34
        public IActionResult Index(string searchString, string cuisine, string priceRange, double? minRating, double? maxDistance, string sortBy)
        {
            var restaurants = _context.Restaurants.AsQueryable();

            // --- Filtros simultáneos ---
            if (!string.IsNullOrEmpty(searchString))
                restaurants = restaurants.Where(r => r.Name.Contains(searchString) || r.City.Contains(searchString));

            if (!string.IsNullOrEmpty(cuisine))
                restaurants = restaurants.Where(r => r.CuisineType == cuisine);

            if (!string.IsNullOrEmpty(priceRange))
                restaurants = restaurants.Where(r => r.PriceRange == priceRange);

            if (minRating.HasValue)
                restaurants = restaurants.Where(r => r.Rating >= minRating.Value);

            if (maxDistance.HasValue)
                restaurants = restaurants.Where(r => r.Distance <= maxDistance.Value);

            // --- Lógica de ordenamiento ---
            restaurants = sortBy switch
            {
                "rating" => restaurants.OrderByDescending(r => r.Rating),
                "price" => restaurants.OrderBy(r => r.PriceRange.Length),
                "distance" => restaurants.OrderBy(r => r.Distance),
                _ => restaurants.OrderBy(r => r.Name)
            };

            // --- Datos para la vista (Solución a errores de ViewBag) ---
            ViewBag.TotalResults = restaurants.Count();
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCuisine = cuisine;
            ViewBag.CurrentPrice = priceRange;

            return View(restaurants.ToList());
        }
    }
}