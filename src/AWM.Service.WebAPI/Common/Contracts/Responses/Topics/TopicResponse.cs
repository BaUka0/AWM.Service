namespace AWM.Service.WebAPI.Common.Contracts.Responses.Topics;

public record TopicResponse(
    long Id,
    long? DirectionId,
    string DirectionTitle,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn,
    int WorkTypeId,
    string WorkTypeName,
    int MaxParticipants,
    int AcceptedApplicationsCount,
    int PendingApplicationsCount,
    string Status,
    string? ReviewComment,
    DateTime CreatedAt);
