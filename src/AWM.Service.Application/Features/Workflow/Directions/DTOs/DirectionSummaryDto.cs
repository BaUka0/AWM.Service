namespace AWM.Service.Application.Features.Workflow.Directions.DTOs;

/// <summary>
/// DTO representing summary information for a thesis direction.
/// </summary>
public record DirectionSummaryDto(
    long Id,
    int OrgUnitId,
    int SemesterId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    int CurrentStateId,
    string CurrentStateName,
    string CurrentStateDisplayName,
    DateTime CreatedAt,
    int CreatedBy,
    string CreatorFullName,
    string CreatorPositionTitle);
