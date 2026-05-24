using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.UpdateSupervisorWorkload;

/// <summary>
/// Validator for UpdateSupervisorWorkloadCommand.
/// </summary>
public sealed class UpdateSupervisorWorkloadCommandValidator : AbstractValidator<UpdateSupervisorWorkloadCommand>
{
    public UpdateSupervisorWorkloadCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0).WithMessage("OrgUnitId must be greater than 0.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");

        RuleFor(x => x.MaxWorkload)
            .GreaterThan(0).WithMessage("MaxWorkload must be greater than 0.");
    }
}
