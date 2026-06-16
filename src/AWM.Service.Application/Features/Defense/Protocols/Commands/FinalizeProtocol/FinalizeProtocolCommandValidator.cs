using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Protocols.Commands.FinalizeProtocol;

public sealed class FinalizeProtocolCommandValidator : AbstractValidator<FinalizeProtocolCommand>
{
    public FinalizeProtocolCommandValidator()
    {
        RuleFor(x => x.ProtocolId).GreaterThan(0);
    }
}
