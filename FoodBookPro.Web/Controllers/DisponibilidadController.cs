using System;
using System.Threading.Tasks;

public class DisponibilidadController : Controller
{
    private readonly IDisponibilidadRepository _disponibilidadRepo;

    public DisponibilidadController(IDisponibilidadRepository disponibilidadRepo)
    {
        _disponibilidadRepo = disponibilidadRepo;
    }

    // GET /Disponibilidad/Configurar
    public async Task<IActionResult> Configurar()
    {
        var configs = await _disponibilidadRepo.ObtenerTodasAsync();

        var vm = new ConfigurarDisponibilidadViewModel
        {
            Configuraciones = configs.Select(c => new DisponibilidadViewModel
            {
                Id = c.Id,
                DiaSemana = c.DiaSemana.ToString(),
                HoraApertura = c.HoraApertura,
                HoraCierre = c.HoraCierre,
                CapacidadMaximaPersonas = c.CapacidadMaximaPersonas,
                MaximaReservasPorFranja = c.MaximaReservasPorFranja,
                ConfirmacionInmediata = c.ConfirmacionInmediata,
                EstaDisponible = c.EstaDisponible
            })
        };
        return View(vm);
    }

    // POST /Disponibilidad/Agregar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(ConfigurarDisponibilidadViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var configs = await _disponibilidadRepo.ObtenerTodasAsync();
            vm.Configuraciones = configs.Select(c => new DisponibilidadViewModel
            {
                Id = c.Id,
                DiaSemana = c.DiaSemana.ToString(),
                HoraApertura = c.HoraApertura,
                HoraCierre = c.HoraCierre,
                CapacidadMaximaPersonas = c.CapacidadMaximaPersonas,
                MaximaReservasPorFranja = c.MaximaReservasPorFranja,
                ConfirmacionInmediata = c.ConfirmacionInmediata,
                EstaDisponible = c.EstaDisponible
            });
            return View("Configurar", vm);
        }

        if (!Enum.TryParse<DayOfWeek>(vm.DiaSemana, out var dia))
        {
            ModelState.AddModelError("DiaSemana", "Día inválido.");
            return View("Configurar", vm);
        }

        var nueva = DisponibilidadRestaurante.Crear(
            dia,
            vm.HoraApertura,
            vm.HoraCierre,
            vm.CapacidadMaximaPersonas,
            vm.MaximaReservasPorFranja,
            vm.ConfirmacionInmediata);

        await _disponibilidadRepo.CrearAsync(nueva);
        TempData["Mensaje"] = $"Disponibilidad para {vm.DiaSemana} guardada.";
        return RedirectToAction(nameof(Configurar));
    }

    // POST /Disponibilidad/Actualizar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Actualizar(ConfigurarDisponibilidadViewModel vm)
    {
        var existente = await _disponibilidadRepo.ObtenerPorDiaAsync(
            Enum.Parse<DayOfWeek>(vm.DiaSemana));

        if (existente is null) return NotFound();

        existente.Actualizar(
            vm.HoraApertura,
            vm.HoraCierre,
            vm.CapacidadMaximaPersonas,
            vm.MaximaReservasPorFranja,
            vm.ConfirmacionInmediata,
            vm.EstaDisponible);

        await _disponibilidadRepo.ActualizarAsync(existente);
        TempData["Mensaje"] = "Configuración actualizada.";
        return RedirectToAction(nameof(Configurar));
    }
}