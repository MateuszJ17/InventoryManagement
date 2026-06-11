using InventoryManagement.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Database.DbContext;

public class InventoryManagementDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public const string ConnectionStringName = "InventoryManagementDb";

    public InventoryManagementDbContext(DbContextOptions<InventoryManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.ProductId);
            e.Property(x => x.ProductId).ValueGeneratedOnAdd();
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.Property(x => x.Description).IsRequired().HasMaxLength(50);
            e.Property(x => x.Price).IsRequired();
            e.Property(x => x.Stock).IsRequired();
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.OrderId);
            e.Property(x => x.OrderId).ValueGeneratedOnAdd();
            e.Property(x => x.TotalValue).IsRequired();
            e.HasMany<Product>(x => x.Products).WithMany();
            e.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.CustomerId).ValueGeneratedOnAdd();
            e.Property(x => x.Name).IsRequired().HasMaxLength(50);
            e.Property(x => x.RegionalCode).IsRequired().HasMaxLength(2);
        });
    }
}