using InventoryManagement.Database.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InventoryManagement.Tests.IntegrationTests;

public class InventoryManagementWebApplicationFactory : WebApplicationFactory<Program>
{
    // prevent tests from interfering with each other data
    private readonly string _databaseName = $"InventoryManagementTests_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            // remove real DI registration to prepare for in memory DB
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(DbContextOptions<InventoryManagementDbContext>) ||
                    d.ServiceType == typeof(InventoryManagementDbContext) ||
                    (d.ServiceType.FullName?.Contains("IDbContextOptionsConfiguration") ?? false))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<InventoryManagementDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        });
    }
}
