using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodBookPro.Test.Controllers;

public class OrdersControllerTests
{
    private static FoodbookDbContext CrearDbContext()
    {
        var options = new DbContextOptionsBuilder<FoodbookDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var db = new FoodbookDbContext(options);
        db.Orders.Add(new Order
        {
            Id = 1,
            Fecha = DateTime.Now,
            Estado = EstadoOrden.Completada,
            RestauranteNombre = "Test",
            Total = 25m,
            Items = new List<OrderItem>
            {
                new() { Id = 1, OrderId = 1, ProductoNombre = "Pizza", Cantidad = 1, Precio = 25m }
            }
        });
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task Index_RetornaVistaConOrdenes()
    {
        using var db = CrearDbContext();
        var controller = new OrdersController(db);

        var result = await controller.Index(null, null, null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<FoodBookPro.Web.Models.OrderListViewModel>(viewResult.Model);
        Assert.Single(model.Orders);
        Assert.Equal("Test", model.Orders[0].RestauranteNombre);
    }

    [Fact]
    public async Task Index_FiltroPorRestaurante_RetornaSoloCoincidentes()
    {
        using var db = CrearDbContext();
        var controller = new OrdersController(db);

        var result = await controller.Index(null, null, null, "Test");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<FoodBookPro.Web.Models.OrderListViewModel>(viewResult.Model);
        Assert.Single(model.Orders);
    }

    [Fact]
    public async Task Detail_OrdenExistente_RetornaVista()
    {
        using var db = CrearDbContext();
        var controller = new OrdersController(db);

        var result = await controller.Detail(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var order = Assert.IsType<Order>(viewResult.Model);
        Assert.Equal(1, order.Id);
        Assert.Single(order.Items);
    }

    [Fact]
    public async Task Detail_OrdenNoExiste_RetornaNotFound()
    {
        using var db = CrearDbContext();
        var controller = new OrdersController(db);

        var result = await controller.Detail(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_PendienteACancelada_ActualizaCorrectamente()
    {
        using var db = CrearDbContext();
        db.Orders.Add(new Order { Id = 2, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "R", Total = 10m });
        await db.SaveChangesAsync();

        var controller = new OrdersController(db);
        var result = await controller.UpdateStatus(2, EstadoOrden.Cancelada);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        var order = await db.Orders.FindAsync(2);
        Assert.Equal(EstadoOrden.Cancelada, order!.Estado);
    }
}
