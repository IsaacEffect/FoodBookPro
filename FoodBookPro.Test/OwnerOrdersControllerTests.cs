using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodBookPro.Test;

/// <summary>5 pruebas XAV-176: Recibir y gestionar órdenes (panel, filtros, detalle, cocina, acciones/timer).</summary>
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
    public async Task Index_XAV176_PanelTiempoReal_DevuelveOrdenes()
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
    public async Task Index_XAV194_FiltroPorEstado()
    {
        using var db = CrearDb();
        db.Orders.Add(new Order { Id = 2, Fecha = DateTime.Now, Estado = EstadoOrden.Completada, RestauranteNombre = "Otro", Total = 10m });
        await db.SaveChangesAsync();
        var ctrl = new OwnerOrdersController(db);
        var r = await ctrl.Index(EstadoOrden.Pendiente, null, null);
        var view = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(r);
        var model = Assert.IsAssignableFrom<FoodBookPro.Web.Models.OwnerOrderListViewModel>(view.Model);
        Assert.Single(model.Orders);
        Assert.Equal(EstadoOrden.Pendiente, model.Orders[0].Estado);
    }

    [Fact]
    public async Task Detail_XAV193_VistaCompletaConItems()
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
    public async Task Kitchen_XAV196_SoloConfirmadasYPreparando()
    {
        using var db = CrearDb();
        db.Orders.Add(new Order { Id = 2, Fecha = DateTime.Now, Estado = EstadoOrden.Confirmada, RestauranteNombre = "C", Total = 20m });
        db.Orders.Add(new Order { Id = 3, Fecha = DateTime.Now, Estado = EstadoOrden.Preparando, RestauranteNombre = "P", Total = 30m });
        db.Orders.Add(new Order { Id = 4, Fecha = DateTime.Now, Estado = EstadoOrden.Completada, RestauranteNombre = "X", Total = 40m });
        await db.SaveChangesAsync();
        var ctrl = new OwnerOrdersController(db);
        var r = await ctrl.Kitchen();
        var view = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(r);
        var list = Assert.IsAssignableFrom<List<Order>>(view.Model);
        Assert.Equal(2, list.Count);
        Assert.All(list, o => Assert.True(o.Estado == EstadoOrden.Confirmada || o.Estado == EstadoOrden.Preparando));
    }

    [Fact]
    public async Task UpdateStatus_XAV195_XAV197_Preparando_AsignaInicioTimer()
    {
        using var db = CrearDb();
        var ctrl = new OwnerOrdersController(db);
        await ctrl.UpdateStatus(1, "preparando");
        var order = await db.Orders.FindAsync(1);
        Assert.Equal(EstadoOrden.Preparando, order!.Estado);
        Assert.NotNull(order.FechaPreparacionInicio);
        Assert.True(order.FechaPreparacionInicio <= DateTime.Now.AddMinutes(1));
    }
}
