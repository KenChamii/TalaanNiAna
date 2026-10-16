using FluentValidation;

namespace TalaanNiAna.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kailangan ang pangalan ng paninda.")
            .MaximumLength(120);

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Kailangan ang kategorya.")
            .MaximumLength(60);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Dapat mas mataas sa zero ang presyo.")
            .LessThanOrEqualTo(1_000_000);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang stock.");

        // Null means "use the store default"; a supplied value must be sensible.
        RuleFor(x => x.LowStockThreshold!.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100_000)
            .When(x => x.LowStockThreshold.HasValue);
    }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kailangan ang pangalan ng paninda.")
            .MaximumLength(120);

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Kailangan ang kategorya.")
            .MaximumLength(60);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Dapat mas mataas sa zero ang presyo.")
            .LessThanOrEqualTo(1_000_000);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang stock.");

        RuleFor(x => x.LowStockThreshold!.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100_000)
            .When(x => x.LowStockThreshold.HasValue);
    }
}
