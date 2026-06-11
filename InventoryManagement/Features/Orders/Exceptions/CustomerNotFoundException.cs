namespace InventoryManagement.Features.Orders.Exceptions;

public class CustomerNotFoundException(Guid customerId) : Exception($"Customer {customerId} not found");