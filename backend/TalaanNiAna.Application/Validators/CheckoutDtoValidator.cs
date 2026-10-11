using FluentValidation;

namespace TalaanNiAna.Application.Validators;

public class CheckoutDtoValidator : AbstractValidator<CheckoutDto>
{
    private static readonly string[] PaymentTypes = ["Cash", "Credit"];

    public CheckoutDtoValidator()
    {
        RuleFor(x => x.PaymentType)
            .NotEmpty().WithMessage("Piliin ang uri ng bayad.")
            .Must(value => PaymentTypes.Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Cash o Credit lang ang tinatanggap.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("Kailangan ang idempotency key.")
            .MaximumLength(64);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Walang laman ang kariton.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0);
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Dapat hindi bababa sa 1 ang dami.");
            item.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang presyo.");
        });

        RuleFor(x => x.CustomerId)
            .NotNull()
            .When(x => string.Equals(x.PaymentType, "Credit", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Piliin kung sino ang uutang.");
    }
}
