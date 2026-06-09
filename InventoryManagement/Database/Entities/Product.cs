namespace InventoryManagement.Database.Entities;

public record Product(Guid ProductId, string Name, string Description, decimal Price, int Stock);