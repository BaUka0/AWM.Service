namespace AWM.Service.Application.Features.Thesis.Works.Commands.CreateStudentWork;

using FluentValidation;

/// <summary>
/// Validator for CreateStudentWorkCommand.
/// </summary>
public sealed class CreateStudentWorkCommandValidator : AbstractValidator<CreateStudentWorkCommand>
{
    public CreateStudentWorkCommandValidator()
    {
        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0)
            .WithMessage("AcademicYearId must be a positive number.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("DepartmentId must be a positive number.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("StudentId must be a positive number.");

        RuleFor(x => x.TopicId)
            .GreaterThan(0)
            .When(x => x.TopicId.HasValue)
            .WithMessage("TopicId must be a positive number when provided.");
    }
}
