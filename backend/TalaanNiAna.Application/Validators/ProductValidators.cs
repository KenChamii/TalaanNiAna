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

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0);
    }
}

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(60);
        RuleFor(x => x.Price).GreaterThan(0).LessThanOrEqualTo(1_000_000);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LowStockThreshold).GreaterThanOrEqualTo(0);
    }
}
