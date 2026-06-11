using InventoryManagement.Database.DbContext;
using InventoryManagement.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Database.Seeder;

public static class DatabaseSeeder
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryManagementDbContext>();
        await dbContext.Database.MigrateAsync();
        await dbContext.SeedAsync();
    }

    private static async Task SeedAsync(this InventoryManagementDbContext dbContext)
    {
        if (dbContext.Customers.Any() || dbContext.Products.Any())
            return;

        List<Customer> customers =
        [
            new("John Doe", "US"),
            new("Jane Doe", "EU"),
            new("Josh Doe", "AS")
        ];

        List<Product> products =
        [
            new("Macbook Pro 15", "Macbook", 1400m, 50),
            new("Wireless mouse", "Wireless", 15m, 150),
            new("Mechanical keyboard", "Mechanical keyboard", 150m, 5),
            new("Gaming monitor", "144p with 144 HZ refresh rate", 500m, 15)
        ];

        await dbContext.Customers.AddRangeAsync(customers);
        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }
}
