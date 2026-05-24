using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.RemoveSupervisor;

public sealed class RemoveSupervisorCommandValidator : AbstractValidator<RemoveSupervisorCommand>
{
    public RemoveSupervisorCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0).WithMessage("OrgUnitId must be greater than 0.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");
    }
}
