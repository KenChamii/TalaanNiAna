using FluentValidation;
using TalaanNiAna.Application.Common;

namespace TalaanNiAna.Application.Validators;

public class StoreSettingsDtoValidator : AbstractValidator<StoreSettingsDto>
{
    public StoreSettingsDtoValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Kailangan ang pangalan ng tindahan.")
            .MaximumLength(120);

        RuleFor(x => x.LowStockAlertLimit)
            .GreaterThanOrEqualTo(0).WithMessage("Hindi pwedeng negatibo ang limit.")
            .LessThanOrEqualTo(100_000);

        RuleFor(x => x.SmsAlertTemplate)
            .MaximumLength(500);

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Kailangan ang timezone.")
            .MaximumLength(64)
            .Must(BeAKnownTimeZone).WithMessage("Hindi kilalang timezone.");
    }

    // Same resolution StoreClock uses, so the two cannot disagree.
    private static bool BeAKnownTimeZone(string timeZoneId)
        => StoreClock.CanResolve(timeZoneId);
}
