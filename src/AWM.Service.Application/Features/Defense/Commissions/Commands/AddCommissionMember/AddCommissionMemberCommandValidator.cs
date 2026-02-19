namespace AWM.Service.Application.Features.Defense.Commissions.Commands.AddCommissionMember;

using FluentValidation;

/// <summary>
/// Validator for AddCommissionMemberCommand.
/// </summary>
public sealed class AddCommissionMemberCommandValidator : AbstractValidator<AddCommissionMemberCommand>
{
    public AddCommissionMemberCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0)
            .WithMessage("Commission ID must be greater than 0.");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.RoleInCommission)
            .IsInEnum()
            .WithMessage("Role in commission must be a valid value.");
    }
}
