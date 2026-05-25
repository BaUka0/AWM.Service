using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.ReconcileTopics;

public sealed class ReconcileTopicsCommandValidator : AbstractValidator<ReconcileTopicsCommand>
{
    public ReconcileTopicsCommandValidator()
    {
        RuleFor(x => x.TopicIds).NotEmpty().WithMessage("At least one topic must be selected.");
        RuleForEach(x => x.TopicIds).GreaterThan(0).WithMessage("Topic ID must be greater than 0.");
    }
}
