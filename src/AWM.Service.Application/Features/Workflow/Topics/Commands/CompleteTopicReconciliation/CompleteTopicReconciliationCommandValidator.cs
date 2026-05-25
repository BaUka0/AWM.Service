using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CompleteTopicReconciliation;

public sealed class CompleteTopicReconciliationCommandValidator : AbstractValidator<CompleteTopicReconciliationCommand>
{
    public CompleteTopicReconciliationCommandValidator()
    {
        RuleFor(x => x.OrgUnitId).GreaterThan(0).WithMessage("OrgUnit ID must be greater than 0.");
        RuleFor(x => x.SemesterId).GreaterThan(0).WithMessage("Semester ID must be greater than 0.");
    }
}
