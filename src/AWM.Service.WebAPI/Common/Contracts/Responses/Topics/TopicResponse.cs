namespace AWM.Service.WebAPI.Common.Contracts.Responses.Topics;

public record TopicResponse(
    long Id,
    long? DirectionId,
    string DirectionTitle,
    int SupervisorId,
    string SupervisorFullName,
    int OrgUnitId,
    int SemesterId,
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
    string CurrentStateName,
    string CurrentStateDisplayName,
    string? ReviewComment,
    DateTime? SubmittedAt,
    DateTime CreatedAt,
    IReadOnlyList<TopicApplicationResponse> Applications);

public record TopicApplicationResponse(
    long Id,
    int StudentId,
    string StudentName,
    string? StudentGroupCode,
    string? StudentSpecialityName,
    int StatusId,
    string StatusText,
    string? MotivationLetter,
    DateTime AppliedAt);
