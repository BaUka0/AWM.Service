using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.DeleteCommission;

/// <summary>
/// Validator for DeleteCommissionCommand.
/// </summary>
public class DeleteCommissionCommandValidator : AbstractValidator<DeleteCommissionCommand>
{
    public DeleteCommissionCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0).WithMessage("CommissionId должен быть больше 0.");
    }
}
