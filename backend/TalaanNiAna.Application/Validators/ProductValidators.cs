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

        // Blank is allowed: loose goods have nothing printed to scan.
        RuleFor(x => x.Barcode!)
            .MaximumLength(32).WithMessage("Masyadong mahaba ang barcode.")
            .Matches(ProductRules.BarcodePattern).WithMessage(ProductRules.BarcodeMessage)
            .When(x => !string.IsNullOrWhiteSpace(x.Barcode));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Dapat mas mataas sa zero ang presyo.")
            .LessThanOrEqualTo(1_000_000);

        // Zero means "not recorded yet". A loss leader is allowed, so cost may
        // exceed price; only negatives are rejected.
        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang puhunan.")
            .LessThanOrEqualTo(1_000_000);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang stock.");

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

        RuleFor(x => x.Barcode!)
            .MaximumLength(32).WithMessage("Masyadong mahaba ang barcode.")
            .Matches(ProductRules.BarcodePattern).WithMessage(ProductRules.BarcodeMessage)
            .When(x => !string.IsNullOrWhiteSpace(x.Barcode));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Dapat mas mataas sa zero ang presyo.")
            .LessThanOrEqualTo(1_000_000);

        RuleFor(x => x.CostPrice!.Value)
            .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang puhunan.")
            .LessThanOrEqualTo(1_000_000)
            .When(x => x.CostPrice.HasValue);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang stock.");

        RuleFor(x => x.LowStockThreshold!.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100_000)
            .When(x => x.LowStockThreshold.HasValue);
    }
}

/// <summary>Shared so create and update cannot drift apart.</summary>
internal static class ProductRules
{
    /// <summary>No spaces: a scanner sends one unbroken token.</summary>
    public const string BarcodePattern = "^[A-Za-z0-9._-]+$";

    public const string BarcodeMessage = "Letra, numero, at - . _ lang ang pwede sa barcode.";
}
