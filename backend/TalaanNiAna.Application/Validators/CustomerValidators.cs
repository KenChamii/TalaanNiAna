using FluentValidation;

namespace TalaanNiAna.Application.Validators;

public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Kailangan ang pangalan ng suki.")
            .MaximumLength(120);

        RuleFor(x => x.ContactNumber)
            .MaximumLength(32)
            .Matches(@"^[0-9+\-\s()]*$").WithMessage("Numero lang ang pwede sa contact number.")
            .When(x => !string.IsNullOrWhiteSpace(x.ContactNumber));
    }
}

public class RecordPaymentDtoValidator : AbstractValidator<RecordPaymentDto>
{
    public RecordPaymentDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Dapat mas mataas sa zero ang bayad.");

        RuleFor(x => x.Note).MaximumLength(250);
    }
}
