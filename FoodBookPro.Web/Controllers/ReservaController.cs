using System;
using System.Threading.Tasks;

public class ReservaController : Controller
{
    private readonly IReservaRepository _reservaRepo;
    private readonly IDisponibilidadRepository _disponibilidadRepo;

    public ReservaController(
        IReservaRepository reservaRepo,
        IDisponibilidadRepository disponibilidadRepo)
    {
        _reservaRepo = reservaRepo;
        _disponibilidadRepo = disponibilidadRepo;
    }

    // ─────────────────────────────────────────────────────────────────
    // CLIENTE — Mis Reservas
    // GET /Reserva/MisReservas
    // ─────────────────────────────────────────────────────────────────
    public async Task<IActionResult> MisReservas()
    {
        // TODO: reemplazar con el Id del usuario autenticado (ClaimsPrincipal)
        var clienteId = ObtenerClienteIdMock();

        var reservas = await _reservaRepo.ObtenerPorClienteAsync(clienteId);
        var vm = reservas.Select(MapToViewModel);
        return View(vm);
    }

    // ─────────────────────────────────────────────────────────────────
    // CLIENTE — Crear Reserva
    // GET /Reserva/Crear
    // ─────────────────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Crear() => View(new CrearReservaViewModel());

    // POST /Reserva/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearReservaViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var disponible = await _reservaRepo.VerificarDisponibilidadAsync(vm.FechaHora, vm.NumeroDPersonas);
        if (!disponible)
        {
            ModelState.AddModelError(string.Empty, "No hay disponibilidad para la fecha, hora y número de personas seleccionados.");
            return View(vm);
        }

        var config = await _disponibilidadRepo.ObtenerPorDiaAsync(vm.FechaHora.DayOfWeek);
        var confirmacionInmediata = config?.ConfirmacionInmediata ?? false;

        // TODO: obtener datos reales del usuario autenticado
        var clienteId = ObtenerClienteIdMock();
        var reserva = Reserva.Crear(
            clienteId,
            nombreCliente: "Cliente Demo",
            emailCliente: "cliente@demo.com",
            telefonoCliente: "000-000-0000",
            vm.FechaHora,
            vm.NumeroDPersonas,
            confirmacionInmediata,
            vm.ComentariosEspeciales);

        await _reservaRepo.CrearAsync(reserva);

        TempData["Mensaje"] = $"Reserva creada exitosamente. Tu código es: {reserva.CodigoReserva}";
        TempData["Codigo"] = reserva.CodigoReserva;
        return RedirectToAction(nameof(Confirmacion), new { id = reserva.Id });
    }

    // GET /Reserva/Confirmacion/{id}
    public async Task<IActionResult> Confirmacion(Guid id)
    {
        var reserva = await _reservaRepo.ObtenerPorIdAsync(id);
        if (reserva is null) return NotFound();
        return View(MapToViewModel(reserva));
    }

    // ─────────────────────────────────────────────────────────────────
    // CLIENTE — Modificar Reserva
    // GET /Reserva/Modificar/{id}
    // ─────────────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Modificar(Guid id)
    {
        var reserva = await _reservaRepo.ObtenerPorIdAsync(id);
        if (reserva is null) return NotFound();

        var vm = new ModificarReservaViewModel
        {
            Id = reserva.Id,
            CodigoReserva = reserva.CodigoReserva,
            FechaHora = reserva.FechaHora,
            NumeroDPersonas = reserva.NumeroDPersonas,
            ComentariosEspeciales = reserva.ComentariosEspeciales
        };
        return View(vm);
    }

    // POST /Reserva/Modificar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Modificar(ModificarReservaViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var reserva = await _reservaRepo.ObtenerPorIdAsync(vm.Id);
        if (reserva is null) return NotFound();

        reserva.Modificar(vm.FechaHora, vm.NumeroDPersonas, vm.ComentariosEspeciales);
        await _reservaRepo.ModificarAsync(reserva);

        TempData["Mensaje"] = "Reserva modificada correctamente.";
        return RedirectToAction(nameof(MisReservas));
    }

    // ─────────────────────────────────────────────────────────────────
    // CLIENTE — Cancelar Reserva
    // POST /Reserva/Cancelar/{id}
    // ─────────────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        await _reservaRepo.CancelarAsync(id);
        TempData["Mensaje"] = "Reserva cancelada.";
        return RedirectToAction(nameof(MisReservas));
    }

    // ─────────────────────────────────────────────────────────────────
    // ADMIN — Panel del Restaurante
    // GET /Reserva/Panel
    // ─────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Panel(DateTime? desde, DateTime? hasta, string? estado)
    {
        EstadoReserva? estadoEnum = estado switch
        {
            "Pendiente" => EstadoReserva.Pendiente,
            "Confirmada" => EstadoReserva.Confirmada,
            "Rechazada" => EstadoReserva.Rechazada,
            "Cancelada" => EstadoReserva.Cancelada,
            _ => null
        };

        var reservas = await _reservaRepo.ObtenerTodasAsync(desde, hasta, estadoEnum);

        var vm = new PanelReservasViewModel
        {
            Reservas = reservas.Select(MapToViewModel),
            FiltroDesde = desde,
            FiltroHasta = hasta,
            FiltroEstado = estado
        };
        return View(vm);
    }

    // ─────────────────────────────────────────────────────────────────
    // ADMIN — Confirmar / Rechazar
    // POST /Reserva/Confirmar/{id}
    // ─────────────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(Guid id)
    {
        await _reservaRepo.ConfirmarAsync(id);
        TempData["Mensaje"] = "Reserva confirmada.";
        return RedirectToAction(nameof(Panel));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rechazar(Guid id)
    {
        await _reservaRepo.RechazarAsync(id);
        TempData["Mensaje"] = "Reserva rechazada.";
        return RedirectToAction(nameof(Panel));
    }

    // ─────────────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────────────
    private static ReservaViewModel MapToViewModel(Reserva r) => new()
    {
        Id = r.Id,
        CodigoReserva = r.CodigoReserva,
        NombreCliente = r.NombreCliente,
        EmailCliente = r.EmailCliente,
        TelefonoCliente = r.TelefonoCliente,
        FechaHora = r.FechaHora,
        NumeroDPersonas = r.NumeroDPersonas,
        ComentariosEspeciales = r.ComentariosEspeciales,
        Estado = r.Estado.ToString(),
        FechaCreacion = r.FechaCreacion
    };

    private static Guid ObtenerClienteIdMock() => Guid.Parse("00000000-0000-0000-0000-000000000001");
}

