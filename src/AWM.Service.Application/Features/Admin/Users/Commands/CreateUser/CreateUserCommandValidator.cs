using FluentValidation;

namespace AWM.Service.Application.Features.Admin.Users.Commands.CreateUser;

/// <summary>
/// Validator for CreateUserCommand.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин обязателен.")
            .MinimumLength(3).WithMessage("Логин должен содержать минимум 3 символа.")
            .MaximumLength(100).WithMessage("Логин не должен превышать 100 символов.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Логин может содержать только латинские буквы, цифры и подчёркивание.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Некорректный формат email.")
            .MaximumLength(255).WithMessage("Email не должен превышать 255 символов.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Роль обязательна.");

        RuleFor(x => x.UniversityId)
            .GreaterThan(0).WithMessage("UniversityId обязателен.");
    }
}
