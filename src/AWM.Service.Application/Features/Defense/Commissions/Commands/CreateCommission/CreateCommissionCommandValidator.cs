using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;

public sealed class CreateCommissionCommandValidator : AbstractValidator<CreateCommissionCommand>
{
    public CreateCommissionCommandValidator()
    {
        RuleFor(x => x.OrgUnitId).GreaterThan(0);
        RuleFor(x => x.SemesterId).GreaterThan(0);
        RuleFor(x => x.SpecialityId).GreaterThan(0).When(x => x.SpecialityId.HasValue);
        RuleFor(x => x.CommissionTypeId).GreaterThan(0);
        RuleFor(x => x.PreDefenseNumber).GreaterThan(0).When(x => x.PreDefenseNumber.HasValue);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(500).When(x => x.Name != null);
        RuleFor(x => x.ChairmanUserId).GreaterThan(0);
        RuleFor(x => x.SecretaryUserId).GreaterThan(0);
        RuleFor(x => x.MemberUserIds).NotNull();
        RuleForEach(x => x.MemberUserIds).GreaterThan(0);
    }
}
