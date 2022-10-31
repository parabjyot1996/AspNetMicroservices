using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder;
public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(rule => rule.UserName)
            .NotEmpty().WithMessage("{UserName} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters.");

        RuleFor(rule => rule.EmailAddress)
            .NotEmpty().WithMessage("{EmailAddress} is required.");

        RuleFor(rule => rule.TotalPrice)
            .NotEmpty().WithMessage("{TotalPrice} is required.")
            .GreaterThan(0).WithMessage("{TotalPrice} should be greater than zero.");
    }
}