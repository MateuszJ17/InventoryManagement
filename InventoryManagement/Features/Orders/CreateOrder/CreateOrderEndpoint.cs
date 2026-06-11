using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Features.Orders.CreateOrder;

[ApiController]
public class CreateOrderEndpoint(IMediator mediator) : ControllerBase
{
    [HttpPost("orders")]
    public async Task<ActionResult<CreateOrderValue>> CreateOrder([FromBody] CreateOrderCommand request)
    {
        var result = await mediator.Send(request);

        if (result is null)
            return BadRequest("Error while creating order. Product stock is too low.");
        
        return Ok(result);
    }
}