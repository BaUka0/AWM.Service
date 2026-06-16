namespace AWM.Service.Application.Features.Workflow.Applications.DTOs;

public record TopicApplicationDto(
    long Id,
    long TopicId,
    string TopicTitleRu,
    string? TopicTitleKz,
    string? TopicTitleEn,
    int StudentId,
    string StudentFullName,
    string StudentGroupCode,
    string? MotivationLetter,
    string Status,
    string? ReviewComment,
    DateTime AppliedAt,
    DateTime? ReviewedAt,
    int? SupervisorId = null,
    string? SupervisorName = null,
    int? WorkTypeId = null,
    string? WorkTypeName = null,
    int? TopicMaxParticipants = null,
    int? TopicAvailableSpots = null,
    string? DirectionTitle = null);
