namespace AWM.Service.Application.Features.Org.Commands.Departments.DeleteDepartment;

using FluentValidation;

/// <summary>
/// Validator for DeleteDepartmentCommand.
/// </summary>
public sealed class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be greater than 0.");
    }
}
