using FluentValidation;

namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseProtocol;

/// <summary>
/// Validator for GeneratePreDefenseProtocolCommand.
/// </summary>
public class GeneratePreDefenseProtocolCommandValidator : AbstractValidator<GeneratePreDefenseProtocolCommand>
{
    public GeneratePreDefenseProtocolCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0).WithMessage("CommissionId должен быть больше 0.");

        RuleFor(x => x.SessionDate)
            .NotEmpty().WithMessage("SessionDate обязателен.");
    }
}
