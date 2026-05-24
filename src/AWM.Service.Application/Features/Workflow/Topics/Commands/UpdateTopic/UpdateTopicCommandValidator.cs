using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.UpdateTopic;

public sealed class UpdateTopicCommandValidator : AbstractValidator<UpdateTopicCommand>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TitleRu).NotEmpty().MaximumLength(500);
        RuleFor(x => x.MaxParticipants).InclusiveBetween(1, 3).When(x => x.MaxParticipants.HasValue);
    }
}
