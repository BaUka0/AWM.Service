namespace AWM.Service.Application.Features.Thesis.Directions.DTOs;

/// <summary>
/// DTO for Direction entity with full details (for detail view).
/// </summary>
public sealed class DirectionDetailDto
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

    /// <summary>
    /// Number of topics created within this direction.
    /// </summary>
    public int TopicsCount { get; init; }
}