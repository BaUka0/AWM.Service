using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.UpdateEmployeeWorkload;

public sealed class UpdateEmployeeWorkloadCommandValidator : AbstractValidator<UpdateEmployeeWorkloadCommand>
{
    public UpdateEmployeeWorkloadCommandValidator()
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
