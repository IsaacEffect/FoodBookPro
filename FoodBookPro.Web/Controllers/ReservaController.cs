using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FoodBookPro.Web.Controllers;

public class ReservaController : Controller
{
    private readonly FoodbookDbContext _context;

    public ReservaController(FoodbookDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var reservas = _context.Reservas.ToList();
        return View(reservas);
    }
}