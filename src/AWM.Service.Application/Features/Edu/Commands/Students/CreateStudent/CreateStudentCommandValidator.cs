namespace AWM.Service.Application.Features.Edu.Commands.Students.CreateStudent;

using FluentValidation;

public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");

        RuleFor(x => x.ProgramId)
            .GreaterThan(0).WithMessage("Academic Program ID must be greater than 0.");

        RuleFor(x => x.AdmissionYear)
            .GreaterThan(2000).WithMessage("Admission year must be greater than 2000.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).WithMessage("Admission year cannot be in the far future.");

        RuleFor(x => x.CurrentCourse)
            .GreaterThan(0).WithMessage("Current course must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Current course cannot exceed 10.");

        RuleFor(x => x.GroupCode)
            .MaximumLength(50).WithMessage("Group code must not exceed 50 characters.")
            .When(x => x.GroupCode is not null);
    }
}
