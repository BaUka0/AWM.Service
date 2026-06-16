using System;

namespace AWM.Service.Application.Features.Workflow.Checks.DTOs;

public enum QualityCheckStatus
{
    Pending = 0,
    Approved = 1,
    SentForRevision = 2
}

public record QualityCheckDto(
    long Id,
    long WorkId,
    int CheckTypeId,
    string CheckTypeName,
    int? AssignedExpertId,
    string? ExpertFullName,
    int AttemptNumber,
    bool IsPassed,
    decimal? ResultValue,
    string? Comment,
    long? AttachmentId,
    DateTime CreatedAt,
    string? StudentName,
    string? TopicTitle,
    string? SubmissionUrl,
    QualityCheckStatus Status = QualityCheckStatus.Pending);
