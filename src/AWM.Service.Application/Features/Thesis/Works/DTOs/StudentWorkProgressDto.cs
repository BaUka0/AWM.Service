namespace AWM.Service.Application.Features.Thesis.Works.DTOs;

using AWM.Service.Domain.Thesis.Entities;

public sealed record StudentWorkProgressDto
{
    public long Id { get; init; }
    public long? TopicId { get; init; }
    public int SemesterId { get; init; }
    public int OrgUnitId { get; init; }
    public int CurrentStateId { get; init; }
    public string? CurrentStateName { get; init; }
    public bool IsDefended { get; init; }
    public string? FinalGrade { get; init; }
    public bool IsEligibleForDefense { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? RepositoryUrl { get; init; }

    public LocalizedTextDto? TopicTitle { get; init; }
    public string? SupervisorName { get; init; }
    public string? SupervisorContacts { get; init; }
    public string? WorkTypeName { get; init; }
    public LocalizedTextDto? DirectionTitle { get; init; }

    public IReadOnlyList<WorkProgressParticipantDto> Participants { get; init; } = [];
    public IReadOnlyList<WorkProgressAttachmentDto> Attachments { get; init; } = [];
    public IReadOnlyList<WorkProgressQualityCheckDto> QualityChecks { get; init; } = [];
    public IReadOnlyList<WorkProgressTimelineItemDto> Timeline { get; init; } = [];
    public IReadOnlyList<WorkProgressNextActionDto> NextActions { get; init; } = [];
}

public sealed record LocalizedTextDto
{
    public string Ru { get; init; } = string.Empty;
    public string? Kk { get; init; }
    public string? En { get; init; }
}

public sealed record WorkProgressParticipantDto
{
    public long Id { get; init; }
    public int StudentId { get; init; }
    public string? Name { get; init; }
    public DateTime JoinedAt { get; init; }
}

public sealed record WorkProgressAttachmentDto
{
    public long Id { get; init; }
    public string FileName { get; init; } = null!;
    public string AttachmentType { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
}

public sealed record WorkProgressQualityCheckDto
{
    public long Id { get; init; }
    public string CheckType { get; init; } = null!;
    public int AttemptNumber { get; init; }
    public bool IsPassed { get; init; }
    public decimal? ResultValue { get; init; }
    public string? Comment { get; init; }
    public DateTime CheckedAt { get; init; }
}

public sealed record WorkProgressTimelineItemDto
{
    public long Id { get; init; }
    public string Type { get; init; } = null!;
    public DateTime Date { get; init; }
    public string Status { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
}

public sealed record WorkProgressNextActionDto
{
    public int TransitionId { get; init; }
    public int ToStateId { get; init; }
    public string ToStateName { get; init; } = null!;
}
