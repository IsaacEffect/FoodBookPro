using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FoodBookPro.Data.Context;

namespace FoodBookPro.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FoodbookDbContext>
    {
        public FoodbookDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FoodbookDbContext>();

            // Usamos la misma cadena de conexión de LocalDB
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=FoodBookProDb;Trusted_Connection=True;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connectionString);

            return new FoodbookDbContext(optionsBuilder.Options);
        }
    }
}