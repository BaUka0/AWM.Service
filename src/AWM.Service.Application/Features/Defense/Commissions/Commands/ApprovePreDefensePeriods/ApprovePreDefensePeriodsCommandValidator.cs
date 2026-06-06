using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.ApprovePreDefensePeriods;

public sealed class ApprovePreDefensePeriodsCommandValidator : AbstractValidator<ApprovePreDefensePeriodsCommand>
{
    public ApprovePreDefensePeriodsCommandValidator()
    {
        RuleFor(x => x.OrgUnitId).GreaterThan(0);
        RuleFor(x => x.SemesterId).GreaterThan(0);
    }
}
