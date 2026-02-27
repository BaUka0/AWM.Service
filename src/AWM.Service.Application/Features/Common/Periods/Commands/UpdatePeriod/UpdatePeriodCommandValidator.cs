namespace AWM.Service.Application.Features.Common.Periods.Commands.UpdatePeriod;

using FluentValidation;

public sealed class UpdatePeriodCommandValidator : AbstractValidator<UpdatePeriodCommand>
{
    public UpdatePeriodCommandValidator()
    {
        RuleFor(x => x.PeriodId)
            .GreaterThan(0).WithMessage("Period ID must be greater than 0.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
