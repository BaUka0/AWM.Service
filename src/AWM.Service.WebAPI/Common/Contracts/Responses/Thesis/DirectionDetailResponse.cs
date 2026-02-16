namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for direction with full details.
/// </summary>
public sealed record DirectionDetailResponse
{
    public long Id { get; init; }
    public int DepartmentId { get; init; }
    public int SupervisorId { get; init; }
    public int AcademicYearId { get; init; }
    public int WorkTypeId { get; init; }

    public string TitleRu { get; init; } = string.Empty;
    public string? TitleKz { get; init; }
    public string? TitleEn { get; init; }
    public string? Description { get; init; }

    public int CurrentStateId { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public int? ReviewedBy { get; init; }
    public string? ReviewComment { get; init; }

    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }

    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
    public int? DeletedBy { get; init; }

    public int TopicsCount { get; init; }
}