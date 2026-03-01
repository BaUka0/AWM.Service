namespace AWM.Service.Application.Features.Edu.Staff.Commands.ApproveSupervisors;

using FluentValidation;

public sealed class ApproveSupervisorsCommandValidator : AbstractValidator<ApproveSupervisorsCommand>
{
    public ApproveSupervisorsCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID is required.");

        RuleFor(x => x.StaffIds)
            .NotEmpty()
            .WithMessage("At least one Staff ID must be provided.");
    }
}
