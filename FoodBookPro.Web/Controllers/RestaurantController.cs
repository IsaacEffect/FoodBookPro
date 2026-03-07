using Microsoft.AspNetCore.Mvc;
using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using System.Linq;

namespace FoodBookPro.Web.Controllers
{
public class RestaurantController : Controller
    {
        private readonly FoodbookDbContext _context;
        private const int PageSize = 10; // Mantenemos el criterio de XAV-26

        // Usamos tu constructor con Inyección de Dependencias (XAV-34)
        public RestaurantController(FoodbookDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString, string cuisine, string priceRange, double? minRating, double? maxDistance, string sortBy, int page = 1)
        {
            var query = _context.Restaurants.AsQueryable();

            // --- 1. Tus Filtros Avanzados (XAV-34) ---
            if (!string.IsNullOrEmpty(searchString))
                query = query.Where(r => r.Name.Contains(searchString) || r.City.Contains(searchString));

            if (!string.IsNullOrEmpty(cuisine))
                query = query.Where(r => r.CuisineType == cuisine);

            if (!string.IsNullOrEmpty(priceRange))
                query = query.Where(r => r.PriceRange == priceRange);

            if (minRating.HasValue)
                query = query.Where(r => r.Rating >= minRating.Value);

            if (maxDistance.HasValue)
                query = query.Where(r => r.Distance <= maxDistance.Value);

            // --- 2. Tu Lógica de Ordenamiento (XAV-34) ---
            query = sortBy switch
            {
                "rating" => query.OrderByDescending(r => r.Rating),
                "price" => query.OrderBy(r => r.PriceRange.Length),
                "distance" => query.OrderBy(r => r.Distance),
                _ => query.OrderBy(r => r.Name)
            };

            // --- 3. Paginación de develop (XAV-26) ---
            var totalItems = query.Count();
            var results = query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // --- 4. Datos para la Vista (ViewBag) ---
            ViewBag.TotalResults = totalItems;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCuisine = cuisine;
            ViewBag.CurrentPrice = priceRange;

            return View(results);
        }
        }
    }
}