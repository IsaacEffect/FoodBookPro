using FoodBookPro.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FoodBookPro.Web.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly FoodbookDbContext _context;
        private const int PageSize = 10;

        public RestaurantController(FoodbookDbContext context)
        {
            _context = context;
        }

        // UN SOLO MÉTODO INDEX: Maneja tanto la carga inicial como los filtros
        public IActionResult Index(string searchString, string cuisine, string priceRange, double? minRating, double? maxDistance, string sortBy, int page = 1)
        {
            var query = _context.Restaurants.AsQueryable();

            // --- 1. Filtros Avanzados (XAV-34) ---
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(r => r.Name.Contains(searchString) || r.City.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(cuisine))
            {
                query = query.Where(r => r.CuisineType == cuisine);
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                query = query.Where(r => r.PriceRange == priceRange);
            }

            if (minRating.HasValue)
            {
                query = query.Where(r => r.Rating >= minRating.Value);
            }

            if (maxDistance.HasValue)
            {
                query = query.Where(r => r.Distance <= maxDistance.Value);
            }

            // --- 2. Lógica de Ordenamiento ---
            query = sortBy switch
            {
                "rating" => query.OrderByDescending(r => r.Rating),
                "price" => query.OrderBy(r => r.PriceRange.Length),
                "distance" => query.OrderBy(r => r.Distance),
                _ => query.OrderBy(r => r.Name)
            };

            // --- 3. Paginación y Conteo ---
            var totalItems = query.Count();

            // Si no hay resultados, al menos pasamos una lista vacía para que la vista no explote
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