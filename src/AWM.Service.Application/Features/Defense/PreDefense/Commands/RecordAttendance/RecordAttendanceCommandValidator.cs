namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.RecordAttendance;

using FluentValidation;

/// <summary>
/// Validator for RecordAttendanceCommand.
/// </summary>
public sealed class RecordAttendanceCommandValidator : AbstractValidator<RecordAttendanceCommand>
{
    public RecordAttendanceCommandValidator()
    {
        RuleFor(x => x.AttemptId)
            .GreaterThan(0)
            .WithMessage("Attempt ID must be greater than 0.");

        RuleFor(x => x.AttendanceStatusId)
            .InclusiveBetween(1, 3)
            .WithMessage("Attendance status must be a valid value (1 = Attended, 2 = Absent, 3 = Excused).");
    }
}
