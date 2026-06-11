namespace InventoryManagement.Features.Orders.Exceptions;

public class ProductNotFoundException(Guid productId) : Exception($"Product {productId} not found");