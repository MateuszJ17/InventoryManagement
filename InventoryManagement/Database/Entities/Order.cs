namespace InventoryManagement.Database.Entities;

public record Order(Guid OrderId, Guid CustomerId, ICollection<Product> Products);