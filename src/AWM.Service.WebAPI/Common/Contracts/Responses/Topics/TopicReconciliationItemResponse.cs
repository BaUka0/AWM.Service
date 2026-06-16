namespace AWM.Service.WebAPI.Common.Contracts.Responses.Topics;

/// <summary>
/// Response for a single topic item in the reconciliation view.
/// </summary>
public record TopicReconciliationItemResponse(
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
