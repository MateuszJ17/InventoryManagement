using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Features.Products.CreateProduct;

[ApiController]
public class CreateProductEndpoint(IMediator mediator) : ControllerBase
{
    [HttpPost("products")]
    public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductCommand request)
    {
        var product = await mediator.Send(request);
        return Ok(product);
    }
}