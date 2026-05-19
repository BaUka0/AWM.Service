namespace AWM.Service.Application.Features.Thesis.Directions.DTOs;

/// <summary>
/// DTO for Direction entity (list view).
/// </summary>
public sealed class DirectionDto
{
    public long Id { get; init; }
    public int OrgUnitId { get; init; }
    public int EmployeeId { get; init; }
    public int SemesterId { get; init; }
    public int WorkTypeId { get; init; }

    public string TitleRu { get; init; } = string.Empty;
    public string? TitleKz { get; init; }
    public string? TitleEn { get; init; }
    public string? DescriptionRu { get; init; }
    public string? DescriptionKz { get; init; }
    public string? DescriptionEn { get; init; }

    public int CurrentStateId { get; init; }
    public string? CurrentStateName { get; init; }
    public string? CurrentStateDisplayName { get; init; }
    public DateTime? SubmittedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public int? ReviewedBy { get; init; }
    public string? ReviewComment { get; init; }

    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
}
