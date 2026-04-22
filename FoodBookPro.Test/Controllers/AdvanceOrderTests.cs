using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using FoodBookPro.Web.Models;
using FoodBookPro.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FoodBookPro.Test.Controllers;

/// <summary>XAV-30: reglas y flujo mínimo de orden anticipada (8 pruebas).</summary>
public class AdvanceOrderTests
{
    private static DateTime PickupTomorrowAfternoon()
    {
        var d = DateTime.Now.AddDays(1);
        return new DateTime(d.Year, d.Month, d.Day, 14, 0, 0);
    }

    [Fact]
    public void ValidateSchedule_RechazaPasadoOMuyCercano()
    {
        var ahora = new DateTime(2025, 3, 1, 12, 0, 0);
        Assert.NotNull(AdvanceOrderRules.ValidateSchedule(ahora.AddMinutes(20), ahora));
    }

    [Fact]
    public void ValidateSchedule_RechazaMasDe14Dias()
    {
        var ahora = new DateTime(2025, 3, 1, 12, 0, 0);
        Assert.NotNull(AdvanceOrderRules.ValidateSchedule(ahora.AddDays(15), ahora));
    }

    [Fact]
    public void ValidateSchedule_AceptaVentanaYHorarioComercial()
    {
        var ahora = new DateTime(2025, 3, 1, 12, 0, 0);
        var retiro = new DateTime(2025, 3, 1, 15, 0, 0);
        Assert.Null(AdvanceOrderRules.ValidateSchedule(retiro, ahora));
    }

    [Fact]
    public void ValidateCartProductNames_RechazaDesconocido()
    {
        Assert.NotNull(AdvanceOrderRules.ValidateCartProductNames(new[] { "Plato inventado" }));
    }

    [Fact]
    public void ValidateCartProductNames_AceptaCatalogo()
    {
        Assert.Null(AdvanceOrderRules.ValidateCartProductNames(new[] { "Pizza Margarita", "Refresco" }));
    }

    [Fact]
    public void Schedule_CarritoVacio_RedirigeACarrito()
    {
        using var db = NewDb();
        var http = NewHttpWithSession();
        var c = NewController(db, new NoOpAdvanceOrderNotifier(), http);
        var r = c.Schedule();
        var red = Assert.IsType<RedirectToActionResult>(r);
        Assert.Equal(nameof(CartController.Index), red.ActionName);
        Assert.Equal("Cart", red.ControllerName);
    }

    [Fact]
    public async Task Place_CreaOrdenNotificadorYLimpiaCarrito()
    {
        using var db = NewDb();
        var notifier = new RecordingNotifier();
        var http = NewHttpWithSession();
        var items = new List<CartItemViewModel> { new() { ProductId = 1, Name = "Pizza Margarita", Price = 10m, Quantity = 1 } };
        http.Session.SetString(CartController.CartSessionKey, JsonSerializer.Serialize(items));
        http.Session.SetString(CartController.RestaurantSessionKey, "Pizza Palace");

        var c = NewController(db, notifier, http);
        var r = await c.Place(PickupTomorrowAfternoon(), "Ana");

        var red = Assert.IsType<RedirectToActionResult>(r);
        Assert.Equal(nameof(OrdersController.Detail), red.ActionName);
        Assert.Equal("Orders", red.ControllerName);

        Assert.Single(db.Orders);
        var o = db.Orders.Include(x => x.Items).First();
        Assert.Equal(EstadoOrden.Pendiente, o.Estado);
        Assert.NotNull(o.HoraRetiro);
        Assert.Single(o.Items);
        Assert.Single(notifier.Placed);

        Assert.True(string.IsNullOrEmpty(http.Session.GetString(CartController.CartSessionKey)));
        Assert.True(string.IsNullOrEmpty(http.Session.GetString(CartController.RestaurantSessionKey)));
    }

    [Fact]
    public async Task Place_HorarioInvalido_RedirigeASchedule()
    {
        using var db = NewDb();
        var http = NewHttpWithSession();
        var items = new List<CartItemViewModel> { new() { ProductId = 1, Name = "Pizza Margarita", Price = 10m, Quantity = 1 } };
        http.Session.SetString(CartController.CartSessionKey, JsonSerializer.Serialize(items));

        var c = NewController(db, new NoOpAdvanceOrderNotifier(), http);
        var r = await c.Place(DateTime.Now.AddMinutes(10), null);

        var red = Assert.IsType<RedirectToActionResult>(r);
        Assert.Equal(nameof(AdvanceOrderController.Schedule), red.ActionName);
    }

    private static FoodbookDbContext NewDb()
    {
        var opt = new DbContextOptionsBuilder<FoodbookDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new FoodbookDbContext(opt);
    }

    private static DefaultHttpContext NewHttpWithSession()
    {
        var http = new DefaultHttpContext();
        http.Features.Set<ISessionFeature>(new TestSessionFeature { Session = new TestSession() });
        return http;
    }

    private static AdvanceOrderController NewController(FoodbookDbContext db, IAdvanceOrderNotifier notifier, DefaultHttpContext http)
    {
        var c = new AdvanceOrderController(db, notifier)
        {
            ControllerContext = new ControllerContext { HttpContext = http },
            TempData = new TempDataDictionary(http, new StubTempDataProvider())
        };
        return c;
    }

    private sealed class StubTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>();
        public void SaveTempData(HttpContext context, IDictionary<string, object?> values) { }
    }

    private sealed class RecordingNotifier : IAdvanceOrderNotifier
    {
        public List<Order> Placed { get; } = new();
        public void NotifyPlaced(Order order) => Placed.Add(order);
    }

    private sealed class TestSessionFeature : ISessionFeature
    {
        public ISession Session { get; set; } = null!;
    }

    private sealed class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _data = new();
        public IEnumerable<string> Keys => _data.Keys;
        public string Id { get; } = "test";
        public bool IsAvailable => true;
        public void Clear() => _data.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _data.Remove(key);
        public void Set(string key, byte[] value) => _data[key] = value;
        public bool TryGetValue(string key, out byte[] value)
        {
            if (_data.TryGetValue(key, out var v)) { value = v; return true; }
            value = Array.Empty<byte>();
            return false;
        }
    }
}
