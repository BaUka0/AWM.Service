namespace AWM.Service.Application.Features.Org.Departments.Commands.UpdateDepartment;

using FluentValidation;

/// <summary>
/// Validator for UpdateDepartmentCommand.
/// </summary>
public sealed class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be greater than 0.");

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
    }
}
