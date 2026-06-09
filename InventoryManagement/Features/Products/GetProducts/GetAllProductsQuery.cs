using InventoryManagement.Database.Entities;
using MediatR;

namespace InventoryManagement.Features.Products.GetProducts;

public record GetAllProductsQuery : IRequest<IReadOnlyCollection<Product>>;