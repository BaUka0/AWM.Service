namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.FinalizeDefense;

using FluentValidation;

/// <summary>
/// Validator for FinalizeDefenseCommand.
/// </summary>
public sealed class FinalizeDefenseCommandValidator : AbstractValidator<FinalizeDefenseCommand>
{
    public FinalizeDefenseCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .GreaterThan(0)
            .WithMessage("Schedule ID must be greater than 0.");
    }
}
