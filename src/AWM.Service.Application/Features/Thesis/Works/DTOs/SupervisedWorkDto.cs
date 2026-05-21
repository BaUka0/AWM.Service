namespace AWM.Service.Application.Features.Thesis.Works.DTOs;

public sealed record SupervisedWorkDto
{
    public long WorkId { get; init; }
    public LocalizedTextDto? TopicTitle { get; init; }
    public LocalizedTextDto? DirectionTitle { get; init; }
    public string? WorkTypeName { get; init; }
    public string? CurrentStateName { get; init; }
    public string? StageKey { get; init; }
    public bool IsDefended { get; init; }
    public string? FinalGrade { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? MetadataJson { get; init; }

    public IReadOnlyList<SupervisedStudentDto> Students { get; init; } = [];
    public IReadOnlyList<WorkProgressAttachmentDto> Attachments { get; init; } = [];
}

public sealed record SupervisedStudentDto
{
    public int StudentId { get; init; }
    public string? Name { get; init; }
    public decimal? Score { get; init; }
}
