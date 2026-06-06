using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Protocols.Commands.CreateProtocol;

public sealed class CreateProtocolCommandValidator : AbstractValidator<CreateProtocolCommand>
{
    public CreateProtocolCommandValidator()
    {
        RuleFor(x => x.ScheduleId).GreaterThan(0);
        RuleFor(x => x.ProtocolNumber).MaximumLength(100).When(x => x.ProtocolNumber != null);
        RuleFor(x => x.FinalScoreNumeric).InclusiveBetween(0, 100).When(x => x.FinalScoreNumeric.HasValue);
        RuleFor(x => x.FinalGradeLetter).MaximumLength(10).When(x => x.FinalGradeLetter != null);
        RuleFor(x => x.Decision).MaximumLength(2000).When(x => x.Decision != null);
        RuleFor(x => x.Comments).MaximumLength(4000).When(x => x.Comments != null);
        RuleFor(x => x.DecisionType).GreaterThan(0).When(x => x.DecisionType.HasValue);
        RuleFor(x => x.ReadinessPercent).InclusiveBetween(0, 100).When(x => x.ReadinessPercent.HasValue);
    }
}
