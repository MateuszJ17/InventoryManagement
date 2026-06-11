namespace InventoryManagement.Database.Entities;

public class Order
{
    // Parameterless constructor required by EF Core: the public constructor takes
    // the 'products' navigation, which EF cannot bind during model creation.
    private Order()
    {
    }

    public Order(Guid customerId, decimal totalValue, ICollection<Product> products)
    {
        CustomerId = customerId;
        TotalValue = totalValue;
        Products = products;
    }

    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalValue { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
