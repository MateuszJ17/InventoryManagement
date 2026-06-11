namespace InventoryManagement.Tests.IntegrationTests;

// All integration test classes share a single application factory and run
// sequentially. This avoids running the application entry point concurrently,
// which is not supported by WebApplicationFactory's HostFactoryResolver.
[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<InventoryManagementWebApplicationFactory>
{
    public const string Name = "Integration tests";
}
