namespace AWM.Service.Application.Features.Thesis.Directions.Commands.SubmitDirection;

using FluentValidation;

/// <summary>
/// Validator for SubmitDirectionCommand.
/// </summary>
public sealed class SubmitDirectionCommandValidator
    : AbstractValidator<SubmitDirectionCommand>
{
    public SubmitDirectionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Direction ID must be greater than 0.");

    }
}