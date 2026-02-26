using FoodBookPro.Web.Models;
using Xunit;

namespace FoodBookPro.Test.Models;

public class CartViewModelTests
{
    [Fact]
    public void Total_SumaSubtotalesDeItems()
    {
        var model = new CartViewModel
        {
            Items = new List<CartItemViewModel>
            {
                new() { Price = 10m, Quantity = 2 },
                new() { Price = 5m, Quantity = 3 }
            }
        };

        Assert.Equal(35m, model.Total);
    }

    [Fact]
    public void Total_ListaVacia_RetornaCero()
    {
        var model = new CartViewModel { Items = new List<CartItemViewModel>() };
        Assert.Equal(0m, model.Total);
    }
}
