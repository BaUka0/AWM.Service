namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.GenerateProtocol;

using FluentValidation;

/// <summary>
/// Validator for GenerateProtocolCommand.
/// </summary>
public sealed class GenerateProtocolCommandValidator : AbstractValidator<GenerateProtocolCommand>
{
    public GenerateProtocolCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .GreaterThan(0)
            .WithMessage("Schedule ID must be greater than 0.");

        RuleFor(x => x.CommissionId)
            .GreaterThan(0)
            .WithMessage("Commission ID must be greater than 0.");
    }
}
