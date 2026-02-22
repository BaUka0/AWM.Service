namespace AWM.Service.Application.Features.Thesis.Applications.Commands.CreateApplication;

using FluentValidation;

/// <summary>
/// Validator for CreateApplicationCommand.
/// </summary>
public sealed class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationCommandValidator()
    {
        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .WithMessage("TopicId must be greater than 0.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("StudentId must be greater than 0.");

        RuleFor(x => x.MotivationLetter)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.MotivationLetter))
            .WithMessage("Motivation letter cannot exceed 2000 characters.");
    }
}