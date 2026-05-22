using FluentValidation;

namespace AWM.Service.Application.Features.Auth.Auth.Commands.RemoveUserAccess;

/// <summary>
/// Validator for RemoveUserAccessCommand.
/// </summary>
public class RemoveUserAccessCommandValidator : AbstractValidator<RemoveUserAccessCommand>
{
    public RemoveUserAccessCommandValidator()
    {
        RuleFor(x => x.UserAccessId)
            .GreaterThan(0).WithMessage("UserAccessId должен быть больше 0.");
    }
}
