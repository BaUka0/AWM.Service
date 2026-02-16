using FluentValidation;

namespace AWM.Service.Application.Features.Auth.Commands.Login;

/// <summary>
/// Validator for LoginCommand.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин обязателен.")
            .MaximumLength(100).WithMessage("Логин не должен превышать 100 символов.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.");
    }
}
