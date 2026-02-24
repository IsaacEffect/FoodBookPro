using FoodBookPro.Data.Context;
using FoodBookPro.Data.Interfaces;
using FoodBookPro.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<FoodbookDbContext>(options =>
            options.UseInMemoryDatabase("FoodbookDb"));

            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
