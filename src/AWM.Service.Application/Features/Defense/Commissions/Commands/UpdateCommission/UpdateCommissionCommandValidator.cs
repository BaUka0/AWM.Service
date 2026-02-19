namespace AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;

using FluentValidation;

/// <summary>
/// Validator for UpdateCommissionCommand.
/// </summary>
public sealed class UpdateCommissionCommandValidator : AbstractValidator<UpdateCommissionCommand>
{
    public UpdateCommissionCommandValidator()
    {
        RuleFor(x => x.CommissionId)
            .GreaterThan(0)
            .WithMessage("Commission ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Commission name is required.")
            .MaximumLength(255)
            .WithMessage("Commission name must not exceed 255 characters.");
    }
}
