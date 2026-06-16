using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.SubmitDirection;

/// <summary>
/// Validator for the <see cref="SubmitDirectionCommand"/>.
/// </summary>
public sealed class SubmitDirectionCommandValidator : AbstractValidator<SubmitDirectionCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitDirectionCommandValidator"/> class.
    /// </summary>
    public SubmitDirectionCommandValidator()
    {
        RuleFor(v => v.DirectionId).GreaterThan(0);
    }
}
