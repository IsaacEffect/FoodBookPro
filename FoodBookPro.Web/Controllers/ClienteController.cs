using FoodBookPro.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodBookPro.Web.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ClienteService _service;

        public ClienteController(ClienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(string nombre, string email, string password, string telefono)
        {
            try
            {
                await _service.RegistrarAsync(nombre, email, password, telefono);
                return Content("Cliente creado correctamente");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        // ===== LOGIN =====

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var cliente = await _service.AutenticarAsync(email, password);

                return Content($"Bienvenido {cliente.Nombre}");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}