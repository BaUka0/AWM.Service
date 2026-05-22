using FluentValidation;

namespace AWM.Service.Application.Features.Auth.Auth.Commands.UpdateRoleOperationAction;

/// <summary>
/// Validator for UpdateRoleOperationActionCommand.
/// </summary>
public class UpdateRoleOperationActionCommandValidator : AbstractValidator<UpdateRoleOperationActionCommand>
{
    public UpdateRoleOperationActionCommandValidator()
    {
        RuleFor(x => x.RoleAccessId)
            .GreaterThan(0).WithMessage("RoleAccessId должен быть больше 0.");

        RuleFor(x => x.RoleOperationId)
            .GreaterThan(0).WithMessage("RoleOperationId должен быть больше 0.");

        RuleFor(x => x.RoleActionTypeId)
            .GreaterThan(0).WithMessage("RoleActionTypeId должен быть больше 0.");
    }
}
