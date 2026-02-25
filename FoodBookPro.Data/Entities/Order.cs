namespace FoodBookPro.Data.Entities
{
    public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } // Ejemplo: "Pendiente de pago"
}
}