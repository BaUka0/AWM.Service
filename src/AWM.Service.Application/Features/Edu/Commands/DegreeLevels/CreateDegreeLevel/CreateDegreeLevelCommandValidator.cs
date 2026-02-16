namespace AWM.Service.Application.Features.Edu.Commands.DegreeLevels.CreateDegreeLevel;

using FluentValidation;

/// <summary>
/// Validator for CreateDegreeLevelCommand.
/// </summary>
public sealed class CreateDegreeLevelCommandValidator 
    : AbstractValidator<CreateDegreeLevelCommand>
{
    public CreateDegreeLevelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Degree level name is required.")
            .MaximumLength(100)
            .WithMessage("Degree level name cannot exceed 100 characters.");

        RuleFor(x => x.DurationYears)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 years.")
            .LessThanOrEqualTo(10)
            .WithMessage("Duration cannot exceed 10 years.");

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0)
            .WithMessage("CreatedBy user ID must be greater than 0.");
    }
}