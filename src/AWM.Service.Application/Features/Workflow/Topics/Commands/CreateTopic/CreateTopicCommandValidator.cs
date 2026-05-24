using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CreateTopic;

public sealed class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(x => x.SemesterId).GreaterThan(0);
        RuleFor(x => x.WorkTypeId).GreaterThan(0);
        RuleFor(x => x.TitleRu).NotEmpty().MaximumLength(500);
        RuleFor(x => x.MaxParticipants).InclusiveBetween(1, 3);
    }
}
