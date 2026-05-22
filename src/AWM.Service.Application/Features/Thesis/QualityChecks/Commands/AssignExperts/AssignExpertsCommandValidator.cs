using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.AssignExperts;

/// <summary>
/// Validator for AssignExpertsCommand.
/// </summary>
public class AssignExpertsCommandValidator : AbstractValidator<AssignExpertsCommand>
{
    public AssignExpertsCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId должен быть больше 0.");

        RuleFor(x => x.Assignments)
            .NotEmpty().WithMessage("Assignments не может быть пустым.");

        RuleForEach(x => x.Assignments)
            .ChildRules(assignment =>
            {
                assignment.RuleFor(a => a.UserId)
                    .GreaterThan(0).WithMessage("UserId должен быть больше 0.");

                assignment.RuleFor(a => a.CheckTypeId)
                    .GreaterThan(0).WithMessage("CheckTypeId должен быть больше 0.");
            });
    }
}
