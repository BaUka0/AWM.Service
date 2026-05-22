using FluentValidation;

namespace AWM.Service.Application.Features.Auth.Auth.Commands.AddUserAccess;

/// <summary>
/// Validator for AddUserAccessCommand.
/// </summary>
public class AddUserAccessCommandValidator : AbstractValidator<AddUserAccessCommand>
{
    public AddUserAccessCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId должен быть больше 0.");

        RuleFor(x => x.RoleAccessId)
            .GreaterThan(0).WithMessage("RoleAccessId должен быть больше 0.");
    }
}
