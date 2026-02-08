namespace AWM.Service.Application.Features.Org.Commands.Departments.UpdateDepartment;

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
            .WithMessage("Department name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(50)
            .WithMessage("Department code must not exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Code));
    }
}
