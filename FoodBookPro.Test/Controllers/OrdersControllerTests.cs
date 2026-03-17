using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodBookPro.Test.Controllers;

/// <summary>Pruebas mínimas para Ver Mis Órdenes (XAV-33).</summary>
public class OrdersControllerTests
{
    private static FoodbookDbContext CrearDb()
    {
        var opt = new DbContextOptionsBuilder<FoodbookDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FoodbookDbContext(opt);
        db.Orders.Add(new Order { Id = 1, Fecha = DateTime.Now, Estado = EstadoOrden.Completada, RestauranteNombre = "Test", Total = 25m, Items = new List<OrderItem> { new() { Id = 1, OrderId = 1, ProductoNombre = "Pizza", Cantidad = 1, Precio = 25m } } });
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task Index_RetornaListaDeOrdenes()
    {
        using var db = CrearDb();
        var ctrl = new OrdersController(db);
        var r = await ctrl.Index(null, null, null, null);
        var view = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(r);
        var model = Assert.IsAssignableFrom<FoodBookPro.Web.Models.OrderListViewModel>(view.Model);
        Assert.Single(model.Orders);
        Assert.Equal("Test", model.Orders[0].RestauranteNombre);
    }

    [Fact]
    public async Task Index_FiltroRestaurante_DevuelveCoincidentes()
    {
        using var db = CrearDb();
        var ctrl = new OrdersController(db);
        var r = await ctrl.Index(null, null, null, "Test");
        var view = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(r);
        var model = Assert.IsAssignableFrom<FoodBookPro.Web.Models.OrderListViewModel>(view.Model);
        Assert.Single(model.Orders);
    }

    [Fact]
    public async Task Detail_OrdenExiste_RetornaVista()
    {
        using var db = CrearDb();
        var ctrl = new OrdersController(db);
        var r = await ctrl.Detail(1);
        var view = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(r);
        var order = Assert.IsType<Order>(view.Model);
        Assert.Equal(1, order.Id);
        Assert.Single(order.Items);
    }

    [Fact]
    public async Task Detail_OrdenNoExiste_NotFound()
    {
        using var db = CrearDb();
        var ctrl = new OrdersController(db);
        var r = await ctrl.Detail(999);
        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(r);
    }

    [Fact]
    public async Task Cancel_Pendiente_CambiaACancelada()
    {
        using var db = CrearDb();
        db.Orders.Add(new Order { Id = 2, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "R", Total = 10m });
        await db.SaveChangesAsync();
        var ctrl = new OrdersController(db);
        var r = await ctrl.Cancel(2);
        Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>(r);
        var order = await db.Orders.FindAsync(2);
        Assert.Equal(EstadoOrden.Cancelada, order!.Estado);
    }
}
