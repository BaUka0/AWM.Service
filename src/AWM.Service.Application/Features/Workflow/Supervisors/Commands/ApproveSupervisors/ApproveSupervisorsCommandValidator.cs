using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.ApproveSupervisors;

public sealed class ApproveSupervisorsCommandValidator : AbstractValidator<ApproveSupervisorsCommand>
{
    public ApproveSupervisorsCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");

        RuleFor(x => x.Assignments)
            .NotNull().WithMessage("Assignments list cannot be null.")
            .Must(x => x != null && x.Select(a => a.UserId).Distinct().Count() == x.Count)
            .WithMessage("Duplicate users in the assignments list are not allowed.");

        RuleForEach(x => x.Assignments).ChildRules(assignments =>
        {
            assignments.RuleFor(a => a.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
            
            assignments.RuleFor(a => a.MaxWorkload)
                .GreaterThan(0).WithMessage("MaxWorkload must be greater than 0.");
        });
    }
}
