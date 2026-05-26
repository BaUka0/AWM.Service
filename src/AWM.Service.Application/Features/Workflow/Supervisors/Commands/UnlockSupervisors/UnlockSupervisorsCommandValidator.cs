using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.UnlockSupervisors;

public sealed class UnlockSupervisorsCommandValidator : AbstractValidator<UnlockSupervisorsCommand>
{
    public UnlockSupervisorsCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0)
            .WithMessage("Organization Unit ID must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0)
            .WithMessage("Semester ID must be greater than 0.");
    }
}
