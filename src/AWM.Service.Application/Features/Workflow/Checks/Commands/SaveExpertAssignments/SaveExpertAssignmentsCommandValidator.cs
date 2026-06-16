using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.SaveExpertAssignments;

public sealed class SaveExpertAssignmentsCommandValidator : AbstractValidator<SaveExpertAssignmentsCommand>
{
    public SaveExpertAssignmentsCommandValidator()
    {
        RuleFor(x => x.OrgUnitId).GreaterThan(0);
        RuleForEach(x => x.Assignments).ChildRules(input =>
        {
            input.RuleFor(x => x.UserId).GreaterThan(0);
            input.RuleFor(x => x.CheckTypeId).GreaterThan(0);
        });
    }
}
