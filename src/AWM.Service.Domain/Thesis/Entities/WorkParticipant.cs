namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// WorkParticipant entity - represents a student participating in a work.
/// </summary>
public class WorkParticipant : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int StudentId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Legacy field
    public DateTime JoinedAt => CreatedAt;

    private WorkParticipant() { }

    internal WorkParticipant(long workId, int studentId, int createdBy = 0)
    {
        WorkId = workId;
        StudentId = studentId;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}

