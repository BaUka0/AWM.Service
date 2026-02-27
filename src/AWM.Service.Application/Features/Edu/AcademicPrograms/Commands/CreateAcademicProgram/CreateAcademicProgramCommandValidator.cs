namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.CreateAcademicProgram;

using FluentValidation;

/// <summary>
/// Validator for CreateAcademicProgramCommand.
/// </summary>
public sealed class CreateAcademicProgramCommandValidator 
    : AbstractValidator<CreateAcademicProgramCommand>
{
    public CreateAcademicProgramCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be greater than 0.");

        RuleFor(x => x.DegreeLevelId)
            .GreaterThan(0)
            .WithMessage("Degree level ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Program name is required.")
            .MaximumLength(200)
            .WithMessage("Program name cannot exceed 200 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(50)
            .WithMessage("Program code cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0)
            .WithMessage("CreatedBy user ID must be greater than 0.");
    }
}