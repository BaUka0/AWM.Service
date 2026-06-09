using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;

public sealed class UpdateCommissionCommandValidator : AbstractValidator<UpdateCommissionCommand>
{
    public UpdateCommissionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(500).When(x => x.Name != null);

        RuleFor(x => x.SecretaryUserId)
            .NotEqual(x => x.ChairmanUserId)
            .When(x => x.SecretaryUserId.HasValue && x.ChairmanUserId.HasValue)
            .WithMessage("Secretary cannot be the same person as Chairman.");

        RuleFor(x => x.MemberUserIds)
            .Must((cmd, memberIds) =>
                memberIds == null ||
                !cmd.ChairmanUserId.HasValue ||
                !memberIds.Contains(cmd.ChairmanUserId.Value))
            .WithMessage("Commission member cannot be the same person as Chairman.")
            .Must((cmd, memberIds) =>
                memberIds == null ||
                !cmd.SecretaryUserId.HasValue ||
                !memberIds.Contains(cmd.SecretaryUserId.Value))
            .WithMessage("Commission member cannot be the same person as Secretary.")
            .Must(m => m == null || m.Count == m.Distinct().Count())
            .WithMessage("Commission members must be unique.");
    }
}
