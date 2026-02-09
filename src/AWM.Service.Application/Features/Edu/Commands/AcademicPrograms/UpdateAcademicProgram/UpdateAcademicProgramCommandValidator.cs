namespace AWM.Service.Application.Features.Edu.Commands.AcademicPrograms.UpdateAcademicProgram;

using FluentValidation;

/// <summary>
/// Validator for UpdateAcademicProgramCommand.
/// </summary>
public sealed class UpdateAcademicProgramCommandValidator 
    : AbstractValidator<UpdateAcademicProgramCommand>
{
    public UpdateAcademicProgramCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Academic program ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Program name is required.")
            .MaximumLength(200)
            .WithMessage("Program name cannot exceed 200 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(50)
            .WithMessage("Program code cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Code));

        RuleFor(x => x.ModifiedBy)
            .GreaterThan(0)
            .WithMessage("ModifiedBy user ID must be greater than 0.");
    }
}