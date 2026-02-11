namespace AWM.Service.Application.Features.Thesis.Topics.Commands.UpdateTopic;

using FluentValidation;

/// <summary>
/// Validator for UpdateTopicCommand.
/// </summary>
public sealed class UpdateTopicCommandValidator : AbstractValidator<UpdateTopicCommand>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .WithMessage("Topic ID must be greater than 0.");

        RuleFor(x => x.TitleRu)
            .NotEmpty()
            .WithMessage("Russian title is required.")
            .MaximumLength(500)
            .WithMessage("Russian title must not exceed 500 characters.");

        RuleFor(x => x.TitleKz)
            .MaximumLength(500)
            .WithMessage("Kazakh title must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TitleKz));

        RuleFor(x => x.TitleEn)
            .MaximumLength(500)
            .WithMessage("English title must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TitleEn));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MaxParticipants)
            .InclusiveBetween(1, 5)
            .WithMessage("Max participants must be between 1 and 5.");

        RuleFor(x => x.ModifiedBy)
            .GreaterThan(0)
            .WithMessage("ModifiedBy must be a valid user ID.");
    }
}