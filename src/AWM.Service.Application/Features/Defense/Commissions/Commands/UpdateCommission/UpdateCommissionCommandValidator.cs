using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;

public sealed class UpdateCommissionCommandValidator : AbstractValidator<UpdateCommissionCommand>
{
    public UpdateCommissionCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(500).When(x => x.Name != null);
    }
}
