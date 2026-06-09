using InventoryManagement.Database.DbContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<InventoryManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(InventoryManagementDbContext.ConnectionStringName)));

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Program>());

// TODO: add validation behavior, global exception handler;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();