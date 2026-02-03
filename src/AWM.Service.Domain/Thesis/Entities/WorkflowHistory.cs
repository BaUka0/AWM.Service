namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// WorkflowHistory entity - logs all state transitions for audit purposes.
/// </summary>
public class WorkflowHistory : Entity<long>, IAuditable
{
    public long WorkId { get; private set; }
    public int? FromStateId { get; private set; }
    public int ToStateId { get; private set; }
    public int UserId { get; private set; }
    public string? Comment { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Legacy field
    public DateTime TransitionDate => CreatedAt;

    private WorkflowHistory() { }

    internal WorkflowHistory(long workId, int? fromStateId, int toStateId, int userId, string? comment = null)
    {
        WorkId = workId;
        FromStateId = fromStateId;
        ToStateId = toStateId;
        UserId = userId;
        Comment = comment;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userId;
    }
}
