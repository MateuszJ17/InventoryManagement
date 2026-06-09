using InventoryManagement.Database.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Features.Products.GetProducts;

[ApiController]
public class GetAllProductsEndpoint(IMediator mediator) : ControllerBase
{
    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyCollection<Product>>> GetAllProducts()
    {
        var products = await mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }
}