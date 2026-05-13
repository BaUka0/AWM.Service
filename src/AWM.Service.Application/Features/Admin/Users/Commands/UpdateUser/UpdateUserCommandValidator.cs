using FluentValidation;

namespace AWM.Service.Application.Features.Admin.Users.Commands.UpdateUser;

/// <summary>
/// Validator for UpdateUserCommand.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId обязателен.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Некорректный формат email.")
            .MaximumLength(255).WithMessage("Email не должен превышать 255 символов.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Роль обязательна.");
    }
}
