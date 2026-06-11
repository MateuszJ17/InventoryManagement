using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Database.DbContext;
using InventoryManagement.Database.Entities;
using InventoryManagement.Features.Orders.CreateOrder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Tests.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class CreateOrderEndpointTests(InventoryManagementWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<(Guid CustomerId, Guid ProductId)> SeedCustomerAndProduct(int stock)
    {
        var customer = new Customer("John Doe", "US") { CustomerId = Guid.NewGuid() };
        var product = new Product("Laptop", "RGB gaming laptop", 1000m, stock) { ProductId = Guid.NewGuid() };

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InventoryManagementDbContext>();
        
        await db.Customers.AddAsync(customer);
        await db.Products.AddAsync(product);
        await db.SaveChangesAsync();

        return (customer.CustomerId, product.ProductId);
    }

    [Fact]
    public async Task CreateOrder_WithValidRequest_ReturnsOkDecrementsStockAndCreatesOrder()
    {
        var (customerId, productId) = await SeedCustomerAndProduct(stock: 5);
        
        var request = new CreateOrderCommand(customerId, [productId]);
        var response = await _client.PostAsJsonAsync("/orders", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CreateOrderValue>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.OrderId);
        Assert.Equal(1000m, result.TotalValue);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InventoryManagementDbContext>();

        var stock = await db.Products
            .Where(p => p.ProductId == productId)
            .Select(p => p.Stock)
            .FirstAsync();

        Assert.Equal(4, stock);

        var orderExists = await db.Orders.AnyAsync(o => o.OrderId == result.OrderId);
        Assert.True(orderExists);
    }

    [Fact]
    public async Task CreateOrder_WithEmptyProductIds_ReturnsBadRequest()
    {
        var customer = new Customer("Jane Doe", "US") { CustomerId = Guid.NewGuid() };

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<InventoryManagementDbContext>();
            await db.Customers.AddAsync(customer);
            await db.SaveChangesAsync();
        }

        var request = new CreateOrderCommand(customer.CustomerId, []);
        var response = await _client.PostAsJsonAsync("/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WithUnknownCustomer_ReturnsNotFound()
    {
        var (_, productId) = await SeedCustomerAndProduct(stock: 5);

        var request = new CreateOrderCommand(Guid.NewGuid(), [productId]);
        var response = await _client.PostAsJsonAsync("/orders", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WithUnknownProduct_ReturnsNotFound()
    {
        var (customerId, _) = await SeedCustomerAndProduct(stock: 5);

        var request = new CreateOrderCommand(customerId, [Guid.NewGuid()]);
        var response = await _client.PostAsJsonAsync("/orders", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_WhenStockTooLow_ReturnsBadRequest()
    {
        var (customerId, productId) = await SeedCustomerAndProduct(stock: 0);
        
        var request = new CreateOrderCommand(customerId, [productId]);
        var response = await _client.PostAsJsonAsync("/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
