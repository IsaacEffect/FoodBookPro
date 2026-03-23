using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodBookPro.Test;

/// <summary>Pruebas mínimas XAV-177: Actualizar estado de orden.</summary>
public class OwnerOrdersControllerTests
{
    private static FoodbookDbContext CrearDb()
    {
        var opt = new DbContextOptionsBuilder<FoodbookDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FoodbookDbContext(opt);
        db.Orders.Add(new Order { Id = 1, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "Test", Total = 50m, HoraRetiro = DateTime.Now.AddHours(1) });
        db.OrderItems.Add(new OrderItem { Id = 1, OrderId = 1, ProductoNombre = "Pizza", Cantidad = 2, Precio = 25m });
        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task Index_DevuelveOrdenes_CuandoHayDatos()
    {
        using var db = CrearDb();
        var ctrl = new OwnerOrdersController(db);
        var r = await ctrl.Index(null, null, null);
        var view = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(r);
        var model = Assert.IsAssignableFrom<FoodBookPro.Web.Models.OwnerOrderListViewModel>(view.Model);
        Assert.Single(model.Orders);
        Assert.Equal("Test", model.Orders[0].RestauranteNombre);
    }

    [Fact]
    public async Task Detail_OrdenExiste_RetornaVista()
    {
        using var db = CrearDb();
        var ctrl = new OwnerOrdersController(db);
        var r = await ctrl.Detail(1);
        var view = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(r);
        var order = Assert.IsType<Order>(view.Model);
        Assert.Equal(1, order.Id);
        Assert.NotEmpty(order.Items);
    }

    [Fact]
    public async Task UpdateStatus_Aceptar_CambiaEstadoYRegistraHistorial()
    {
        using var db = CrearDb();
        var ctrl = new OwnerOrdersController(db);
        await ctrl.UpdateStatus(1, "aceptar");

        var order = await db.Orders.FindAsync(1);
        Assert.Equal(EstadoOrden.Confirmada, order!.Estado);

        var hist = await db.OrderStatusHistories.Where(h => h.OrderId == 1).ToListAsync();
        Assert.Single(hist);
        Assert.Equal(EstadoOrden.Pendiente, hist[0].EstadoAnterior);
        Assert.Equal(EstadoOrden.Confirmada, hist[0].EstadoNuevo);
        Assert.True(hist[0].NotificadoCliente);
    }

    [Fact]
    public async Task ConfigurarTiempoEstimado_GuardaMinutos()
    {
        using var db = CrearDb();
        var ctrl = new OwnerOrdersController(db);
        await ctrl.ConfigurarTiempoEstimado(1, 30);

        var order = await db.Orders.FindAsync(1);
        Assert.Equal(30, order!.TiempoEstimadoPreparacionMinutos);
    }

    [Fact]
    public async Task HistorialEstados_RetornaLista()
    {
        using var db = CrearDb();
        db.OrderStatusHistories.Add(new OrderStatusHistory { OrderId = 1, EstadoAnterior = EstadoOrden.Pendiente, EstadoNuevo = EstadoOrden.Confirmada, NotificadoCliente = true });
        await db.SaveChangesAsync();

        var ctrl = new OwnerOrdersController(db);
        var r = await ctrl.HistorialEstados(1);
        var json = Assert.IsType<Microsoft.AspNetCore.Mvc.JsonResult>(r);
        Assert.NotNull(json.Value);
    }
}
