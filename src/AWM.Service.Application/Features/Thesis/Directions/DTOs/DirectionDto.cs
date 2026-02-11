namespace AWM.Service.Application.Features.Thesis.Directions.DTOs;

/// <summary>
/// DTO for Direction entity (list view).
/// </summary>
public sealed class DirectionDto
{
    public long Id { get; init; }
    public int DepartmentId { get; init; }
    public int SupervisorId { get; init; }
    public int AcademicYearId { get; init; }
    public int WorkTypeId { get; init; }

    public string TitleRu { get; init; } = string.Empty;
    public string? TitleKz { get; init; }
    public string? TitleEn { get; init; }

    public int CurrentStateId { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public int? ReviewedBy { get; init; }

    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
}