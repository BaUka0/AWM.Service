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
        RuleFor(x => x.SecretaryUserId).GreaterThan(0)
            .NotEqual(x => x.ChairmanUserId).WithMessage("Secretary cannot be the same person as Chairman.");
        
        RuleFor(x => x.MemberUserIds).NotNull();
        RuleForEach(x => x.MemberUserIds).GreaterThan(0);
        
        RuleFor(x => x.MemberUserIds)
            .Must((cmd, memberIds) => memberIds == null || !memberIds.Contains(cmd.ChairmanUserId))
            .WithMessage("Commission member cannot be the same person as Chairman.")
            .Must((cmd, memberIds) => memberIds == null || !memberIds.Contains(cmd.SecretaryUserId))
            .WithMessage("Commission member cannot be the same person as Secretary.")
            .Must(m => m == null || m.Count == m.Distinct().Count())
            .WithMessage("Commission members must be unique.");
    }
}
