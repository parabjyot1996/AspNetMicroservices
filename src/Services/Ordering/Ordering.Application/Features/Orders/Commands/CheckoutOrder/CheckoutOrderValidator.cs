using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;
public class CheckoutOrderValidator : AbstractValidator<CheckoutOrderCommand>
{
    public CheckoutOrderValidator()
    {
        RuleFor(rule => rule.UserName)
            .NotEmpty().WithMessage("{UserName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{UserName} must be not exceed 50 characters.");

        RuleFor(rule => rule.EmailAddress)
               .NotEmpty().WithMessage("{EmailAddress} is required.");

        RuleFor(rule => rule.TotalPrice)
            .NotEmpty().WithMessage("{TotalPrice} is required.")
            .GreaterThan(0).WithMessage("{TotalPrice} should be greater than zero.");
    }
}