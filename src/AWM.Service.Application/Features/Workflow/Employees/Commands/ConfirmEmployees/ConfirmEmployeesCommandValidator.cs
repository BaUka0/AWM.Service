using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.ConfirmEmployees;

public sealed class ConfirmEmployeesCommandValidator : AbstractValidator<ConfirmEmployeesCommand>
{
    public ConfirmEmployeesCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0)
            .WithMessage("Organization Unit ID must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0)
            .WithMessage("Semester ID must be greater than 0.");
    }
}
