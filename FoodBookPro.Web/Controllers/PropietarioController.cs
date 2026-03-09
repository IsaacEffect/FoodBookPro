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
    }
}