namespace AWM.Service.Application.Features.Thesis.Directions.Commands.CreateDirection;

using FluentValidation;

/// <summary>
/// Validator for CreateDirectionCommand.
/// </summary>
public sealed class CreateDirectionCommandValidator 
    : AbstractValidator<CreateDirectionCommand>
{
    public CreateDirectionCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be greater than 0.");

        RuleFor(x => x.SupervisorId)
            .GreaterThan(0)
            .WithMessage("Supervisor ID must be greater than 0.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0)
            .WithMessage("Academic year ID must be greater than 0.");

        RuleFor(x => x.WorkTypeId)
            .GreaterThan(0)
            .WithMessage("Work type ID must be greater than 0.");

        RuleFor(x => x.TitleRu)
            .NotEmpty()
            .WithMessage("Russian title is required.")
            .MaximumLength(500)
            .WithMessage("Russian title cannot exceed 500 characters.");

        RuleFor(x => x.TitleKz)
            .MaximumLength(500)
            .WithMessage("Kazakh title cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.TitleKz));

        RuleFor(x => x.TitleEn)
            .MaximumLength(500)
            .WithMessage("English title cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.TitleEn));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}