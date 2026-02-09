using Microsoft.EntityFrameworkCore;

namespace FoodBookPro.Data.Context
{
    public class FoodbookDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("FoodbookDb");
        }
    }
}
