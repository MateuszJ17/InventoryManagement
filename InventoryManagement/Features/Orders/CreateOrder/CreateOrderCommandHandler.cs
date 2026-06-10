using InventoryManagement.Database.DbContext;
using InventoryManagement.Database.Entities;
using InventoryManagement.Features.Orders.CalculatePrice;
using InventoryManagement.Features.Orders.Exceptions;
using InventoryManagement.Infrastructure.Date;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Features.Orders.CreateOrder;

public class CreateOrderCommandHandler(
    InventoryManagementDbContext dbContext,
    IFinalPriceCalculator finalPriceCalculator,
    IDateProvider dateProvider)
    : IRequestHandler<CreateOrderCommand, CreateOrderValue?>
{
    public async Task<CreateOrderValue?> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .Where(x => request.ProductsIds.Contains(x.ProductId))
            .ToListAsync(cancellationToken);
        
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId, cancellationToken);

        if (customer is null)
            throw new CustomerNotFoundException(request.CustomerId);
        
        if (products.Any(x => x.Stock < 1))
            return null;

        foreach (var product in products)
        {
            product.Stock--;
        }

        var total = products.Sum(x => x.Price);
        var adjustedTotal = finalPriceCalculator.CalculateFinalPrice(
            total,
            products.Count,
            customer.RegionalCode,
            dateProvider.GetToday(),
            products.Select(x => x.Price).ToArray());
        
        var newOrder = new Order(Guid.NewGuid(), request.CustomerId, adjustedTotal, products);
        var createdOrder = await dbContext.Orders.AddAsync(newOrder, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderValue(createdOrder.Entity.OrderId, total);
    }
}