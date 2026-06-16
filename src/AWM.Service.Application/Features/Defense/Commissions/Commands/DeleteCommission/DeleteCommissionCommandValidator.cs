using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.DeleteCommission;

public sealed class DeleteCommissionCommandValidator : AbstractValidator<DeleteCommissionCommand>
{
    public DeleteCommissionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
