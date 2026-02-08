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
            .WithMessage("Department name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(50)
            .WithMessage("Department code must not exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Code));
    }
}
