namespace AWM.Service.Application.Features.Workflow.Directions.DTOs;

/// <summary>
/// Data transfer object for detailed thesis direction information.
/// </summary>
public record DirectionDto(
    long Id,
    int OrgUnitId,
    int SemesterId,
    int WorkTypeId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn,
    int CurrentStateId,
    DateTime? SubmittedAt,
    DateTime? ReviewedAt,
    int? ReviewedBy,
    string? ReviewComment,
    DateTime CreatedAt,
    int CreatedBy,
    string CreatorFullName,
    string CreatorPositionTitle);
