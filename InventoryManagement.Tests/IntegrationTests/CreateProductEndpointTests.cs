using System.Net;
using System.Net.Http.Json;
using InventoryManagement.Database.DbContext;
using InventoryManagement.Features.Products.CreateProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Tests.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class CreateProductEndpointTests(InventoryManagementWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateProduct_WithValidRequest_ReturnsOkAndPersistsProduct()
    {
        var request = new CreateProductCommand("Laptop", "RGB gaming laptop", 1200m, 10);
        var response = await _client.PostAsJsonAsync("/products", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var productId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, productId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InventoryManagementDbContext>();

        var saved = await db.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

        Assert.NotNull(saved);
        Assert.Equal("Laptop", saved.Name);
        Assert.Equal(1200m, saved.Price);
        Assert.Equal(10, saved.Stock);
    }

    [Theory]
    [InlineData("", "Description", 10, 5)]
    [InlineData("Name", "", 10, 5)]
    [InlineData("Name", "Description", 0, 5)]
    [InlineData("Name", "Description", 10, -1)]
    public async Task CreateProduct_WithInvalidRequest_ReturnsBadRequest(
        string name,
        string description,
        decimal price,
        int stock)
    {
        var request = new CreateProductCommand(name, description, price, stock);
        var response = await _client.PostAsJsonAsync("/products", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
