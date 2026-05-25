namespace AWM.Service.Application.Features.Workflow.Topics.DTOs;

/// <summary>
/// DTO for a single topic in the reconciliation view.
/// Includes supervisor info and application statistics for department decision-making.
/// </summary>
public record TopicReconciliationItemDto(
    long Id,
    long? DirectionId,
    string DirectionTitle,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    int WorkTypeId,
    string WorkTypeName,
    int? SpecialityId,
    int MaxParticipants,
    int AcceptedApplicationsCount,
    int PendingApplicationsCount,
    int TotalApplicationsCount,
    string Status,
    string? ReviewComment,
    string SupervisorFullName,
    int CreatedBy,
    DateTime CreatedAt);
