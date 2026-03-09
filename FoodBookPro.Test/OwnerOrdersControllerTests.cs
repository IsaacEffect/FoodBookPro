using FoodBookPro.Data.Context;
using FoodBookPro.Data.Entities;
using FoodBookPro.Web.Controllers;
using FoodBookPro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Test
{
    /// <summary>
    /// Pruebas unitarias para OwnerOrdersController (XAV-176 Recibir y Gestionar Ordenes)
    /// Patron AAA (Arrange, Act, Assert)
    /// </summary>
    public class OwnerOrdersControllerTests
    {
        private static FoodbookDbContext CrearDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<FoodbookDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new FoodbookDbContext(options);
        }

        private static void SeedOrdenTest(FoodbookDbContext db)
        {
            if (db.Orders.Any()) return;
            var order = new Order
            {
                Id = 1,
                Fecha = DateTime.Now,
                Estado = EstadoOrden.Pendiente,
                RestauranteNombre = "Test Rest",
                Total = 50m,
                HoraRetiro = DateTime.Now.AddHours(1)
            };
            db.Orders.Add(order);
            db.OrderItems.Add(new OrderItem { Id = 1, OrderId = 1, ProductoNombre = "Pizza", Cantidad = 2, Precio = 25m });
            db.SaveChanges();
        }

        /// <summary>
        /// Criterio: Panel de ordenes en tiempo real
        /// </summary>
        [Fact]
        public async Task Index_DevuelveVistaConOrdenes_CuandoHayOrdenes()
        {
            var db = CrearDbContext("Index_1");
            SeedOrdenTest(db);
            var controller = new OwnerOrdersController(db);

            var result = await controller.Index(null, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<OwnerOrderListViewModel>(viewResult.Model);
            Assert.Single(model.Orders);
            Assert.Equal("Test Rest", model.Orders[0].RestauranteNombre);
        }

        /// <summary>
        /// Criterio: Filtros por estado y hora de retiro (XAV-194)
        /// </summary>
        [Fact]
        public async Task Index_FiltraPorEstado_CuandoSeEspecificaEstado()
        {
            var db = CrearDbContext("Index_Filtro_1");
            db.Orders.Add(new Order { Id = 1, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "A", Total = 10m });
            db.Orders.Add(new Order { Id = 2, Fecha = DateTime.Now, Estado = EstadoOrden.Completada, RestauranteNombre = "B", Total = 20m });
            db.SaveChanges();
            var controller = new OwnerOrdersController(db);

            var result = await controller.Index(EstadoOrden.Pendiente, null, null);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<OwnerOrderListViewModel>(viewResult.Model);
            Assert.Single(model.Orders);
            Assert.Equal(EstadoOrden.Pendiente, model.Orders[0].Estado);
        }

        /// <summary>
        /// Criterio: Vista de detalles completos de cada orden (XAV-193)
        /// </summary>
        [Fact]
        public async Task Detail_DevuelveOrdenConItems_CuandoOrdenExiste()
        {
            var db = CrearDbContext("Detail_1");
            SeedOrdenTest(db);
            var controller = new OwnerOrdersController(db);

            var result = await controller.Detail(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var order = Assert.IsType<Order>(viewResult.Model);
            Assert.Equal(1, order.Id);
            Assert.NotEmpty(order.Items);
            Assert.Equal("Pizza", order.Items.First().ProductoNombre);
        }

        /// <summary>
        /// Criterio: Vista de detalles completos (XAV-193)
        /// </summary>
        [Fact]
        public async Task Detail_DevuelveNotFound_CuandoOrdenNoExiste()
        {
            var db = CrearDbContext("Detail_404");
            var controller = new OwnerOrdersController(db);

            var result = await controller.Detail(999);

            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Criterio: Botones de accion - Aceptar (XAV-195)
        /// </summary>
        [Fact]
        public async Task UpdateStatus_Aceptar_CambiaEstadoAConfirmada()
        {
            var db = CrearDbContext("Update_Aceptar");
            SeedOrdenTest(db);
            var controller = new OwnerOrdersController(db);

            var result = await controller.UpdateStatus(1, "aceptar");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Detail", redirect.ActionName);
            var order = await db.Orders.FindAsync(1);
            Assert.Equal(EstadoOrden.Confirmada, order!.Estado);
        }

        /// <summary>
        /// Criterio: Botones de accion - Rechazar (XAV-195)
        /// </summary>
        [Fact]
        public async Task UpdateStatus_Rechazar_CambiaEstadoACancelada()
        {
            var db = CrearDbContext("Update_Rechazar");
            SeedOrdenTest(db);
            var controller = new OwnerOrdersController(db);

            await controller.UpdateStatus(1, "rechazar");

            var order = await db.Orders.FindAsync(1);
            Assert.Equal(EstadoOrden.Cancelada, order!.Estado);
        }

        /// <summary>
        /// Criterio: Botones de accion - Marcar Preparando (XAV-195) + Timer (XAV-197)
        /// </summary>
        [Fact]
        public async Task UpdateStatus_Preparando_AsignaFechaPreparacionInicio()
        {
            var db = CrearDbContext("Update_Preparando");
            SeedOrdenTest(db);
            var controller = new OwnerOrdersController(db);
            var antes = DateTime.Now;

            await controller.UpdateStatus(1, "preparando");

            var order = await db.Orders.FindAsync(1);
            Assert.Equal(EstadoOrden.Preparando, order!.Estado);
            Assert.NotNull(order.FechaPreparacionInicio);
            Assert.True(order.FechaPreparacionInicio >= antes.AddSeconds(-1));
            Assert.True(order.FechaPreparacionInicio <= DateTime.Now.AddSeconds(1));
        }

        /// <summary>
        /// Criterio: Botones de accion - Marcar Lista (XAV-195)
        /// </summary>
        [Fact]
        public async Task UpdateStatus_Lista_CambiaEstadoALista()
        {
            var db = CrearDbContext("Update_Lista");
            SeedOrdenTest(db);
            var order = await db.Orders.FindAsync(1);
            order!.Estado = EstadoOrden.Preparando;
            order.FechaPreparacionInicio = DateTime.Now.AddMinutes(-5);
            await db.SaveChangesAsync();
            var controller = new OwnerOrdersController(db);

            await controller.UpdateStatus(1, "lista");

            var actualizada = await db.Orders.FindAsync(1);
            Assert.Equal(EstadoOrden.Lista, actualizada!.Estado);
        }

        /// <summary>
        /// Criterio: Vista de cocina optimizada (XAV-196)
        /// </summary>
        [Fact]
        public async Task Kitchen_MuestraSoloConfirmadasYPreparando()
        {
            var db = CrearDbContext("Kitchen_1");
            db.Orders.Add(new Order { Id = 1, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "P", Total = 10m });
            db.Orders.Add(new Order { Id = 2, Fecha = DateTime.Now, Estado = EstadoOrden.Confirmada, RestauranteNombre = "C", Total = 20m });
            db.Orders.Add(new Order { Id = 3, Fecha = DateTime.Now, Estado = EstadoOrden.Preparando, RestauranteNombre = "Prep", Total = 30m });
            db.Orders.Add(new Order { Id = 4, Fecha = DateTime.Now, Estado = EstadoOrden.Completada, RestauranteNombre = "Done", Total = 40m });
            db.SaveChanges();
            var controller = new OwnerOrdersController(db);

            var result = await controller.Kitchen();

            var viewResult = Assert.IsType<ViewResult>(result);
            var orders = Assert.IsAssignableFrom<List<Order>>(viewResult.Model);
            Assert.Equal(2, orders.Count);
            Assert.Contains(orders, o => o.Estado == EstadoOrden.Confirmada);
            Assert.Contains(orders, o => o.Estado == EstadoOrden.Preparando);
            Assert.DoesNotContain(orders, o => o.Estado == EstadoOrden.Pendiente);
            Assert.DoesNotContain(orders, o => o.Estado == EstadoOrden.Completada);
        }

        /// <summary>
        /// Criterio: Panel en tiempo real - conteo de pendientes
        /// </summary>
        [Fact]
        public async Task GetPendingCount_DevuelveConteoCorrecto()
        {
            var db = CrearDbContext("Count_1");
            db.Orders.Add(new Order { Id = 1, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "A", Total = 10m });
            db.Orders.Add(new Order { Id = 2, Fecha = DateTime.Now, Estado = EstadoOrden.Pendiente, RestauranteNombre = "B", Total = 20m });
            db.Orders.Add(new Order { Id = 3, Fecha = DateTime.Now, Estado = EstadoOrden.Completada, RestauranteNombre = "C", Total = 30m });
            db.SaveChanges();
            var controller = new OwnerOrdersController(db);

            var result = await controller.GetPendingCount();

            var jsonResult = Assert.IsType<JsonResult>(result);
            var value = jsonResult.Value;
            var countProp = value?.GetType().GetProperty("count");
            Assert.NotNull(countProp);
            Assert.Equal(2, countProp.GetValue(value));
        }

        /// <summary>
        /// Criterio: Botones de accion - validacion
        /// </summary>
        [Fact]
        public async Task UpdateStatus_AccionInvalida_DevuelveBadRequest()
        {
            var db = CrearDbContext("Update_Inv");
            SeedOrdenTest(db);
            var controller = new OwnerOrdersController(db);

            var result = await controller.UpdateStatus(1, "accion_invalida");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// Criterio: Botones de accion - orden no encontrada
        /// </summary>
        [Fact]
        public async Task UpdateStatus_OrdenInexistente_DevuelveNotFound()
        {
            var db = CrearDbContext("Update_404");
            var controller = new OwnerOrdersController(db);

            var result = await controller.UpdateStatus(999, "aceptar");

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
