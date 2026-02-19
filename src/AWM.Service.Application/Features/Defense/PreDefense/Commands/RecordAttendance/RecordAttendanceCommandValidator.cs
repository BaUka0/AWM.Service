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

        RuleFor(x => x.AttendanceStatus)
            .IsInEnum()
            .WithMessage("Attendance status must be a valid value (Attended, Absent, Excused).");
    }
}
