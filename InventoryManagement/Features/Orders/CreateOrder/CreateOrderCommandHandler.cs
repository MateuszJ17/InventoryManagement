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
        var customer = await dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId, cancellationToken);

        if (customer is null)
        {
            logger.LogError("Customer not found: {CustomerId}", request.CustomerId);
            throw new CustomerNotFoundException(request.CustomerId);
        }
        
        var products = await dbContext.Products
            .Where(x => request.ProductsIds.Contains(x.ProductId))
            .ToDictionaryAsync(x => x.ProductId, x => x, cancellationToken);

        var productsQuantities = request.ProductsIds
            .GroupBy(productId => productId)
            .ToDictionary(group => group.Key, group => group.Count());

        foreach (var productsQuantity in productsQuantities)
        {
            if (!products.TryGetValue(productsQuantity.Key, out var product))
            {
                logger.LogWarning("Product {ProductId} not found in inventory", productsQuantity.Key);
                throw new ProductNotFoundException(productsQuantity.Key);
            }

            if (product.Stock < productsQuantity.Value)
            {
                logger.LogWarning("Product {ProductId} has stock too low to place order", productsQuantity.Key);
                return null;
            }
        }

        foreach (var productsQuantity in productsQuantities)
        {
            products[productsQuantity.Key].Stock -= productsQuantity.Value;
        }

        var prices = request.ProductsIds.Select(productId => products[productId].Price).ToArray();
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