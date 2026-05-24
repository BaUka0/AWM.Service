using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.RemoveSupervisor;

public sealed class RemoveSupervisorCommandValidator : AbstractValidator<RemoveSupervisorCommand>
{
    public RemoveSupervisorCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than 0.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");
    }
}
