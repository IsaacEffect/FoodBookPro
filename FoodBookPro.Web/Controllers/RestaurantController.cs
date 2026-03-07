using Microsoft.AspNetCore.Mvc;
using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using System.Linq;

namespace FoodBookPro.Web.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly FoodbookDbContext _context;
        private const int PageSize = 10; // Criterio XAV-26: 10 resultados por página

        public RestaurantController()
        {
            _context = new FoodbookDbContext();

            // ¡CRÍTICO! Esta línea carga los 12 restaurantes del Seed Data 
            // en la base de datos en memoria.
            _context.Database.EnsureCreated();
        }

        public IActionResult Index(string searchTerm, string location, int page = 1)
        {
            var query = _context.Restaurants.AsQueryable();

            // Criterio XAV-26: Búsqueda por nombre (Case-Insensitive)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(term));
            }

            // Criterio XAV-26: Búsqueda por ciudad/zona (Case-Insensitive)
            if (!string.IsNullOrEmpty(location))
            {
                var loc = location.ToLower();
                query = query.Where(r => r.City.ToLower().Contains(loc));
            }

            // Lógica de Paginación funcional
            var totalItems = query.Count();
            var results = query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)System.Math.Ceiling(totalItems / (double)PageSize);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Location = location;

            return View(results);
        }
    }
}