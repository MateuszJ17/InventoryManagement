using InventoryManagement.Database.DbContext;
using InventoryManagement.Database.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Features.Products.GetProducts;

public class GetAllProductsQueryHandler(InventoryManagementDbContext dbContext)
    : IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<Product>>
{
    public async Task<IReadOnlyCollection<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // TODO: add logging
        
        var products = await dbContext.Products.ToListAsync(cancellationToken: cancellationToken);
        return products;
    }
}