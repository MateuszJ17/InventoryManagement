using FluentValidation;
using InventoryManagement.Features.Orders.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Infrastructure.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var details = exception switch
        {
            ValidationException ve => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Detail = string.Join(", ", ve.Errors.Select(e => e.ErrorMessage))
            },
            CustomerNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Customer not found",
                Detail = exception.Message
            },
            ProductNotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Product not found",
                Detail = exception.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred"
            }
        };

        context.Response.StatusCode = details.Status!.Value;
        await context.Response.WriteAsJsonAsync(details, cancellationToken);
        return true;
    }
}