namespace AWM.Service.Application.Features.Workflow.Directions.DTOs;

/// <summary>
/// DTO representing summary information for a thesis direction.
/// </summary>
public record DirectionSummaryDto(
    long Id,
    int OrgUnitId,
    int SemesterId,
    int WorkTypeId,
    int SupervisorId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn,
    int CurrentStateId,
    string CurrentStateName,
    string CurrentStateDisplayName,
    DateTime CreatedAt,
    int CreatedBy,
    string CreatorFullName,
    string CreatorPositionTitle,
    string SupervisorFullName,
    string? ReviewComment = null);
