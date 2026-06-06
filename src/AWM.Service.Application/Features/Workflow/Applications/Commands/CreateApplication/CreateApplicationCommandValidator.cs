using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.CreateApplication;

public sealed class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationCommandValidator()
    {
        RuleFor(x => x.TopicId).GreaterThan(0);
        RuleFor(x => x.MotivationLetter).MaximumLength(4000).When(x => x.MotivationLetter != null);
    }
}
