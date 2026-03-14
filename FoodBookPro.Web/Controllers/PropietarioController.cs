using FoodBookPro.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodBookPro.Web.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly PropietarioService _service;

        public PropietarioController(PropietarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(
            string nombre,
            string email,
            string password,
            string telefono,
            string restaurante)
        {
            try
            {
                await _service.RegistrarAsync(
                    nombre,
                    email,
                    password,
                    telefono,
                    restaurante
                );

                return Content("Solicitud enviada. Estado: Pendiente");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Aprobar(Guid id)
        {
            try
            {
                await _service.AprobarPropietarioAsync(id);
                return Content("Propietario aprobado");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Rechazar(Guid id)
        {
            try
            {
                await _service.RechazarPropietarioAsync(id);
                return Content("Propietario rechazado");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

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
                var propietario = await _service.AutenticarAsync(email, password);

                return Content($"Bienvenido propietario {propietario.Nombre}");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}