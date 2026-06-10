namespace InventoryManagement.Database.Entities;

public class Product(Guid productId, string name, string description, decimal price, int stock)
{
    public Guid ProductId { get; set; } = productId;
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;
    public decimal Price { get; set; } = price;
    public int Stock { get; set; } = stock;
}