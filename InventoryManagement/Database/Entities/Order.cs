namespace InventoryManagement.Database.Entities;

public class Order(Guid orderId, Guid customerId, decimal totalValue, ICollection<Product> products)
{
    public Guid OrderId { get; set; } = orderId;
    public Guid CustomerId { get; set; } = customerId;
    public decimal TotalValue { get; set; } = totalValue;
    public ICollection<Product> Products { get; set; } = products;
}