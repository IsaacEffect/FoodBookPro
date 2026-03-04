using FoodBookPro.Web.Models;
using Xunit;

namespace FoodBookPro.Test.Models;

public class CartItemViewModelTests
{
    [Fact]
    public void Subtotal_CalculaCorrectamente_PrecioPorCantidad()
    {
        var item = new CartItemViewModel
        {
            Price = 10.50m,
            Quantity = 3
        };

        var subtotal = item.Subtotal;

        Assert.Equal(31.50m, subtotal);
    }

    [Fact]
    public void Subtotal_CantidadCero_RetornaCero()
    {
        var item = new CartItemViewModel { Price = 5m, Quantity = 0 };
        Assert.Equal(0m, item.Subtotal);
    }
}
