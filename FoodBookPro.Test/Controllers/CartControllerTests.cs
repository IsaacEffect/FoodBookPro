using FoodBookPro.Web.Controllers;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FoodBookPro.Test.Controllers;

/// <summary>5 pruebas XAV-31: carrito (modelo + sesión).</summary>
public class CartControllerTests
{
    private static DefaultHttpContext NewHttp()
    {
        var http = new DefaultHttpContext();
        http.Features.Set<ISessionFeature>(new TestSessionFeature { Session = new TestSession() });
        return http;
    }

    private static CartController Ctrl(DefaultHttpContext http) =>
        new() { ControllerContext = new ControllerContext { HttpContext = http } };

    [Fact]
    public void CartViewModel_Total_SumaSubtotales()
    {
        var m = new CartViewModel
        {
            Items = new List<CartItemViewModel>
            {
                new() { Price = 10m, Quantity = 2 },
                new() { Price = 5m, Quantity = 3 }
            }
        };
        Assert.Equal(35m, m.Total);
    }

    [Fact]
    public void CartItemViewModel_Subtotal_PrecioPorCantidad()
    {
        var i = new CartItemViewModel { Price = 4m, Quantity = 3 };
        Assert.Equal(12m, i.Subtotal);
    }

    [Fact]
    public void AddItem_GuardaProductoEnSesion()
    {
        var http = NewHttp();
        Ctrl(http).AddItem(1, "Pizza", 10m, 2);
        var vm = Assert.IsType<CartViewModel>(((ViewResult)Ctrl(http).Index()).Model!);
        Assert.Single(vm.Items);
        Assert.Equal(2, vm.Items[0].Quantity);
        Assert.Equal("Pizza", vm.Items[0].Name);
    }

    [Fact]
    public void Remove_EliminaItemPorIndice()
    {
        var http = NewHttp();
        var c = Ctrl(http);
        c.AddItem(1, "A", 1m, 1);
        c.AddItem(2, "B", 2m, 1);
        c.Remove(0);
        var vm = Assert.IsType<CartViewModel>(((ViewResult)Ctrl(http).Index()).Model!);
        Assert.Single(vm.Items);
        Assert.Equal("B", vm.Items[0].Name);
    }

    [Fact]
    public void UpdateQuantity_CambiaCantidadEnSesion()
    {
        var http = NewHttp();
        var c = Ctrl(http);
        c.AddItem(1, "X", 5m, 2);
        c.UpdateQuantity(0, 4);
        var vm = Assert.IsType<CartViewModel>(((ViewResult)Ctrl(http).Index()).Model!);
        Assert.Equal(4, vm.Items[0].Quantity);
        Assert.Equal(20m, vm.Total);
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
