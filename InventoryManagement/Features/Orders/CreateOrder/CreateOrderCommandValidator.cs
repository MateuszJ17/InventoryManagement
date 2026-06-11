using FluentValidation;

namespace InventoryManagement.Features.Orders.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ProductsIds).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
    }
}