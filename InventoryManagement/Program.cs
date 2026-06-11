using FluentValidation;
using InventoryManagement.Database.DbContext;
using InventoryManagement.Database.Seeder;
using InventoryManagement.Features.Orders.CreateOrder;
using InventoryManagement.Features.Products.CreateProduct;
using InventoryManagement.Features.Orders.CalculateDiscount;
using InventoryManagement.Features.Orders.CalculatePrice;
using InventoryManagement.Infrastructure.Date;
using InventoryManagement.Infrastructure.Exceptions;
using InventoryManagement.Infrastructure.Validation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));

    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

    builder.Services.AddDbContext<InventoryManagementDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString(InventoryManagementDbContext.ConnectionStringName)));

    builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Program>());
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    builder.Services.AddScoped<IValidator<CreateProductCommand>, CreateProductCommandValidator>();
    builder.Services.AddScoped<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
    builder.Services.AddScoped<IDateProvider, DateProvider>();
    builder.Services.AddScoped<IHolidaysDaysProvider, HolidaysDaysProvider>();
    builder.Services.AddScoped<IDiscountCalculator, DiscountCalculator>();
    builder.Services.AddScoped<IFinalPriceCalculator, FinalPriceCalculator>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    if (!app.Environment.IsEnvironment("Testing"))
    {
        await app.MigrateAndSeedAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException) // when clause needed because of test host shenanigans
{
    Log.Fatal(ex, "Application failed unexpectedly during startup");
}
finally
{
    Log.CloseAndFlush();
}

// needed for integration tests
public partial class Program;

