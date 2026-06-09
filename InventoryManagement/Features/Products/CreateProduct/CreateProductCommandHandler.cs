using InventoryManagement.Database.DbContext;
using InventoryManagement.Database.Entities;
using MediatR;

namespace InventoryManagement.Features.Products.CreateProduct;

public class CreateProductCommandHandler(InventoryManagementDbContext dbContext)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // TODO: add logging
        
        var newProduct = new Product(request.ProductId, request.Name, request.Description, request.Price, request.Stock);
        var insertResult = await dbContext.Products.AddAsync(newProduct, cancellationToken);

        return insertResult.Entity.ProductId;
    }
}