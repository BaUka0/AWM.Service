using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.RemoveEmployee;

public sealed class RemoveEmployeeCommandValidator : AbstractValidator<RemoveEmployeeCommand>
{
    public RemoveEmployeeCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0).WithMessage("OrgUnitId must be greater than 0.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");
    }
}
