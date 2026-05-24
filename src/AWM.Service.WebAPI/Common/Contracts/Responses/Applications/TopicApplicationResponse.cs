namespace AWM.Service.WebAPI.Common.Contracts.Responses.Applications;

public record TopicApplicationResponse(
    long Id,
    long TopicId,
    string TopicTitle,
    int StudentId,
    string StudentName,
    string StudentGroupCode,
    string? MotivationLetter,
    string Status,
    string? ReviewComment,
    DateTime AppliedAt,
    DateTime? ReviewedAt);
