namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

using AWM.Service.WebAPI.Common.Contracts.Responses.Common;

public sealed record StudentWorkProgressResponse
{
    public long Id { get; init; }
    public long? TopicId { get; init; }
    public int AcademicYearId { get; init; }
    public int DepartmentId { get; init; }
    public int CurrentStateId { get; init; }
    public string? CurrentStateName { get; init; }
    public bool IsDefended { get; init; }
    public string? FinalGrade { get; init; }
    public bool IsEligibleForDefense { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? MetadataJson { get; init; }

    public LocalizedTextResponse? TopicTitle { get; init; }
    public string? SupervisorName { get; init; }
    public string? SupervisorContacts { get; init; }
    public string? WorkTypeName { get; init; }
    public LocalizedTextResponse? DirectionTitle { get; init; }

    public IReadOnlyList<WorkProgressParticipantResponse> Participants { get; init; } = [];
    public IReadOnlyList<WorkProgressAttachmentResponse> Attachments { get; init; } = [];
    public IReadOnlyList<WorkProgressQualityCheckResponse> QualityChecks { get; init; } = [];
    public IReadOnlyList<WorkProgressTimelineItemResponse> Timeline { get; init; } = [];
    public IReadOnlyList<WorkProgressNextActionResponse> NextActions { get; init; } = [];
    public IReadOnlyList<PendingCheckResponse> PendingChecks { get; init; } = [];
}

public sealed record PendingCheckResponse
{
    public int CheckTypeId { get; init; }
    public string Title { get; init; } = null!;
    public string? Code { get; init; }
}

public sealed record WorkProgressParticipantResponse
{
    public long Id { get; init; }
    public int StudentId { get; init; }
    public string? Name { get; init; }
    public DateTime JoinedAt { get; init; }
}

public sealed record WorkProgressAttachmentResponse
{
    public long Id { get; init; }
    public string FileName { get; init; } = null!;
    public string AttachmentType { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
}

public sealed record WorkProgressQualityCheckResponse
{
    public long Id { get; init; }
    public string CheckType { get; init; } = null!;
    public int AttemptNumber { get; init; }
    public bool IsPassed { get; init; }
    public decimal? ResultValue { get; init; }
    public string? Comment { get; init; }
    public DateTime CheckedAt { get; init; }
}

public sealed record WorkProgressTimelineItemResponse
{
    public long Id { get; init; }
    public string Type { get; init; } = null!;
    public DateTime Date { get; init; }
    public string Status { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
}

public sealed record WorkProgressNextActionResponse
{
    public int TransitionId { get; init; }
    public int ToStateId { get; init; }
    public string ToStateName { get; init; } = null!;
}
