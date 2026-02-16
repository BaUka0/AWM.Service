namespace AWM.Service.Application.Features.Edu.Commands.Students.UpdateStudent;

using FluentValidation;

public sealed class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Student ID must be greater than 0.");

        RuleFor(x => x.GroupCode)
            .MaximumLength(50).WithMessage("Group code must not exceed 50 characters.")
            .When(x => x.GroupCode is not null);

        RuleFor(x => x.CurrentCourse)
            .GreaterThan(0).WithMessage("Current course must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Current course cannot exceed 10.")
            .When(x => x.CurrentCourse.HasValue);

        RuleFor(x => x.ProgramId)
            .GreaterThan(0).WithMessage("Academic Program ID must be greater than 0.")
            .When(x => x.ProgramId.HasValue);
    }
}
