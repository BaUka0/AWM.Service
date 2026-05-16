namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

using AWM.Service.WebAPI.Common.Contracts.Responses.Common;

public sealed record SupervisedWorkResponse
{
    public long WorkId { get; init; }
    public LocalizedTextResponse? TopicTitle { get; init; }
    public LocalizedTextResponse? DirectionTitle { get; init; }
    public string? WorkTypeName { get; init; }
    public string? CurrentStateName { get; init; }
    public string? StageKey { get; init; }
    public bool IsDefended { get; init; }
    public string? FinalGrade { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? RepositoryUrl { get; init; }

    public IReadOnlyList<SupervisedStudentResponse> Students { get; init; } = [];
    public IReadOnlyList<WorkProgressAttachmentResponse> Attachments { get; init; } = [];
}

public sealed record SupervisedStudentResponse
{
    public int StudentId { get; init; }
    public string? Name { get; init; }
    public string Role { get; init; } = null!;
    public bool IsLeader { get; init; }
    public decimal? Score { get; init; }
}
