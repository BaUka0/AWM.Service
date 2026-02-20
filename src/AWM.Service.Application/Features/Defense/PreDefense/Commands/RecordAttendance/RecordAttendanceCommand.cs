namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.RecordAttendance;

using AWM.Service.Domain.Defense.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a secretary to record a student's attendance for a pre-defense attempt.
/// </summary>
public sealed record RecordAttendanceCommand : IRequest<Result>
{
    /// <summary>
    /// PreDefenseAttempt ID to update.
    /// </summary>
    public long AttemptId { get; init; }

    /// <summary>
    /// Attendance status to record.
    /// </summary>
    public AttendanceStatus AttendanceStatus { get; init; }

    /// <summary>
    /// If absent, whether the absence is excused.
    /// </summary>
    public bool IsExcused { get; init; }
}
