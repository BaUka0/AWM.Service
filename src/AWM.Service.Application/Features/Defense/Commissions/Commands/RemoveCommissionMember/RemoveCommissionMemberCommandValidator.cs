namespace AWM.Service.Application.Features.Defense.Commissions.Commands.RemoveCommissionMember;

using FluentValidation;

/// <summary>
/// Validator for RemoveCommissionMemberCommand.
/// </summary>
public sealed class RemoveCommissionMemberCommandValidator : AbstractValidator<RemoveCommissionMemberCommand>
{
    public RemoveCommissionMemberCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0)
            .WithMessage("Commission ID must be greater than 0.");

        RuleFor(x => x.MemberId)
            .GreaterThan(0)
            .WithMessage("Member ID must be greater than 0.");
    }
}
