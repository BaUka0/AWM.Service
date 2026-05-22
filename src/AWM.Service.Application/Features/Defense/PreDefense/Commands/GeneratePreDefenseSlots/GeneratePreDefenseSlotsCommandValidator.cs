using FluentValidation;

namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseSlots;

/// <summary>
/// Validator for GeneratePreDefenseSlotsCommand.
/// </summary>
public class GeneratePreDefenseSlotsCommandValidator : AbstractValidator<GeneratePreDefenseSlotsCommand>
{
    public GeneratePreDefenseSlotsCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0).WithMessage("CommissionId должен быть больше 0.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date обязателен.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime обязателен.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime обязателен.")
            .GreaterThan(x => x.StartTime).WithMessage("EndTime должен быть больше StartTime.");

        RuleFor(x => x.SlotDurationMinutes)
            .GreaterThan(0).WithMessage("SlotDurationMinutes должен быть больше 0.");
    }
}
