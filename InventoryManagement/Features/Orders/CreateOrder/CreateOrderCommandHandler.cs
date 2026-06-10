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
    IDateProvider dateProvider,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, CreateOrderValue?>
{
    public async Task<CreateOrderValue?> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .Where(x => request.ProductsIds.Contains(x.ProductId))
            .ToDictionaryAsync(x => x.ProductId, x => x, cancellationToken);
        
        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId, cancellationToken);

        if (customer is null)
        {
            logger.LogError("Customer not found: {CustomerId}", request.CustomerId);
            throw new CustomerNotFoundException(request.CustomerId);
        }

        foreach (var productId in request.ProductsIds)
        {
            if (!products.TryGetValue(productId, out var product))
            {
                logger.LogWarning("Product {ProductId} not found in inventory", productId);
                throw new ProductNotFoundException(productId);
            }
            
            if (product.Stock < 1)
            {
                logger.LogWarning("Product {ProductId} has stock too low to place order", productId);
                return null;
            }
            
            products[productId].Stock--;
        }

        var prices = products.Values.Select(x => x.Price).ToArray();
        var total = prices.Sum();
        var adjustedTotal = finalPriceCalculator.CalculateFinalPrice(
            total,
            request.ProductsIds.Count,
            customer.RegionalCode,
            dateProvider.GetToday(),
            prices);
        
        logger.LogInformation(
            "Total value before applying discounts/regional pricing: {Total}, after: {AdjustedTotal}",
            total,
            adjustedTotal);
        
        var newOrder = new Order(request.CustomerId, adjustedTotal, products.Values);
        var createdOrder = await dbContext.Orders.AddAsync(newOrder, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Order created: {OrderId}", createdOrder.Entity.OrderId);

        return new CreateOrderValue(createdOrder.Entity.OrderId, adjustedTotal);
    }
}