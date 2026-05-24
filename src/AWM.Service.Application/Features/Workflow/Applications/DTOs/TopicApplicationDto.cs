namespace AWM.Service.Application.Features.Workflow.Applications.DTOs;

public record TopicApplicationDto(
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
