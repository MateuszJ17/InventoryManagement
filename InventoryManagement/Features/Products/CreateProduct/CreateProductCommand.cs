using MediatR;

namespace InventoryManagement.Features.Products.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock) : IRequest<Guid>;