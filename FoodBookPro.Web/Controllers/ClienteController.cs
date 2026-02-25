using FoodBookPro.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FoodBookPro.Web.Controllers
{
    public class ClienteController : Controller
    {
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(string nombre, string email, string password, string telefono)
        {
            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Email = email,
                Telefono = telefono,
                PasswordHash = password
            };

            //en proceso

            return Content("Cliente creado");
        }
    }
}
