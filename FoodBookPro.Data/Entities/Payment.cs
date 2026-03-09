namespace FoodBookPro.Data.Entities;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TipAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public bool IsSuccess { get; set; }

    public decimal TotalAmount => Subtotal + TipAmount;
}
