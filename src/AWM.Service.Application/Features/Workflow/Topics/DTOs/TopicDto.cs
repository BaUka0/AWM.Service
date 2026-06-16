namespace AWM.Service.Application.Features.Workflow.Topics.DTOs;

public record TopicDto(
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
    IReadOnlyList<TopicApplicationDto> Applications);

public record TopicApplicationDto(
    long Id,
    int StudentId,
    string StudentName,
    string? StudentGroupCode,
    string? StudentSpecialityName,
    int StatusId,
    string StatusText,
    string? MotivationLetter,
    DateTime AppliedAt);
