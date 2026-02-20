namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;

using AWM.Service.Domain.Defense.Enums;
using FluentValidation;

/// <summary>
/// Validator for CreateCommissionCommand.
/// </summary>
public sealed class CreateCommissionCommandValidator : AbstractValidator<CreateCommissionCommand>
{
    public CreateCommissionCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be greater than 0.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0)
            .WithMessage("Academic year ID must be greater than 0.");

        RuleFor(x => x.CommissionType)
            .IsInEnum()
            .WithMessage("Commission type must be a valid value (PreDefense or GAK).");

        RuleFor(x => x.PreDefenseNumber)
            .InclusiveBetween(1, 3)
            .WithMessage("Pre-defense number must be 1, 2, or 3.")
            .When(x => x.PreDefenseNumber.HasValue);

        RuleFor(x => x.Name)
            .MaximumLength(255)
            .WithMessage("Commission name must not exceed 255 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}
