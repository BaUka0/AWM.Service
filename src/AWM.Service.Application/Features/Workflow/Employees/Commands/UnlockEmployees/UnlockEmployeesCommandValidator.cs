using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.UnlockEmployees;

public sealed class UnlockEmployeesCommandValidator : AbstractValidator<UnlockEmployeesCommand>
{
    public UnlockEmployeesCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0)
            .WithMessage("Organization Unit ID must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0)
            .WithMessage("Semester ID must be greater than 0.");
    }
}
