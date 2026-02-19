namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.FinalizePreDefense;

using FluentValidation;

/// <summary>
/// Validator for FinalizePreDefenseCommand.
/// </summary>
public sealed class FinalizePreDefenseCommandValidator : AbstractValidator<FinalizePreDefenseCommand>
{
    public FinalizePreDefenseCommandValidator()
    {
        RuleFor(x => x.AttemptId)
            .GreaterThan(0)
            .WithMessage("Attempt ID must be greater than 0.");

        RuleFor(x => x.AverageScore)
            .InclusiveBetween(0m, 100m)
            .WithMessage("Average score must be between 0 and 100.");
    }
}
