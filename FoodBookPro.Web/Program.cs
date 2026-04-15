using FoodBookPro.Data.Context;
using FoodBookPro.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //a
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<FoodbookDbContext>(options =>
                options.UseInMemoryDatabase("FoodbookDb"));
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddSingleton<IAdvanceOrderNotifier, NoOpAdvanceOrderNotifier>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<FoodbookDbContext>().SeedData();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
