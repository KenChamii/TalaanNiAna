using FluentValidation;

namespace TalaanNiAna.Application.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Kailangan ang username.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Kailangan ang password.");
    }
}

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Kailangan ang username.")
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Kailangan ang password.")
            .MinimumLength(6).WithMessage("Hindi bababa sa 6 na karakter ang password.")
            .MaximumLength(128);

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Kailangan ang pangalan mo.")
            .MaximumLength(120);

        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Kailangan ang pangalan ng tindahan.")
            .MaximumLength(120);
    }
}
