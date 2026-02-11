namespace AWM.Service.Application.Features.Org.Commands.Departments.CreateDepartment;

using FluentValidation;

/// <summary>
/// Validator for CreateDepartmentCommand.
/// </summary>
public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.InstituteId)
            .GreaterThan(0)
            .WithMessage("Institute ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(200)
            .WithMessage("Department name must not exceed 200 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-\.,']+$")
            .WithMessage("Department name contains invalid characters.");

        RuleFor(x => x.Code)
            .MaximumLength(20)
            .WithMessage("Department code must not exceed 20 characters.")
            .Matches(@"^[A-Z0-9\-]*$")
            .WithMessage("Department code must contain only uppercase letters, numbers, and hyphens.")
            .When(x => !string.IsNullOrWhiteSpace(x.Code));

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0)
            .WithMessage("CreatedBy must be a valid user ID.");
    }
}
