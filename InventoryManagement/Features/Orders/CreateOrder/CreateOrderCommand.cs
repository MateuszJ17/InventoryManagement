using MediatR;

namespace InventoryManagement.Features.Orders.CreateOrder;

public record CreateOrderCommand(Guid CustomerId, IReadOnlyCollection<Guid> ProductsIds) : IRequest<CreateOrderValue?>;
