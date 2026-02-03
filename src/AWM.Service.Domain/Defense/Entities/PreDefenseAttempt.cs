namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;

/// <summary>
/// PreDefenseAttempt entity - tracks student's pre-defense attempts.
/// </summary>
public class PreDefenseAttempt : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int PreDefenseNumber { get; private set; }
    public long? ScheduleId { get; private set; }
    public AttendanceStatus AttendanceStatus { get; private set; }
    public decimal? AverageScore { get; private set; }
    public bool IsPassed { get; private set; }
    public DateTime AttemptDate { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private PreDefenseAttempt() { }

    public PreDefenseAttempt(
        long workId,
        int preDefenseNumber,
        int createdBy,
        long? scheduleId = null,
        AttendanceStatus attendanceStatus = AttendanceStatus.Attended)
    {
        if (preDefenseNumber < 1 || preDefenseNumber > 3)
            throw new ArgumentException("Pre-defense number must be 1, 2, or 3.", nameof(preDefenseNumber));

        WorkId = workId;
        PreDefenseNumber = preDefenseNumber;
        ScheduleId = scheduleId;
        AttendanceStatus = attendanceStatus;
        IsPassed = false;
        AttemptDate = DateTime.UtcNow;

        CreatedAt = AttemptDate;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Records the result of the pre-defense.
    /// </summary>
    public void RecordResult(decimal averageScore, bool isPassed, int modifiedBy)
    {
        if (AttendanceStatus != AttendanceStatus.Attended)
            throw new InvalidOperationException("Cannot record result for non-attended attempt.");

        AverageScore = averageScore;
        IsPassed = isPassed;
        
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Marks the student as absent.
    /// </summary>
    public void MarkAbsent(int modifiedBy, bool excused = false)
    {
        AttendanceStatus = excused ? AttendanceStatus.Excused : AttendanceStatus.Absent;
        IsPassed = false;
        AverageScore = null;

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Checks if student needs to retake.
    /// </summary>
    public bool NeedsRetake => !IsPassed && PreDefenseNumber < 3;
}
