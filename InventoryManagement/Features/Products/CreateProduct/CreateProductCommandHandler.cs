using InventoryManagement.Database.DbContext;
using InventoryManagement.Database.Entities;
using MediatR;

namespace InventoryManagement.Features.Products.CreateProduct;

public class CreateProductCommandHandler(
    InventoryManagementDbContext dbContext,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var newProduct = new Product(request.Name, request.Description, request.Price, request.Stock);
        var insertResult = await dbContext.Products.AddAsync(newProduct, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Product created: {ProductId}", insertResult.Entity.ProductId);

        return insertResult.Entity.ProductId;
    }
}