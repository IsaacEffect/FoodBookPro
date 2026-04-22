namespace FoodBookPro.Web.Models;

public class AdvanceScheduleViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public string RestauranteNombre { get; set; } = string.Empty;
}
