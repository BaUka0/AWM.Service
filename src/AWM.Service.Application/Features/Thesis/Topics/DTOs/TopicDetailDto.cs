namespace AWM.Service.Application.Features.Thesis.Topics.DTOs;

/// <summary>
/// Data Transfer Object for Topic entity with full details and applications.
/// </summary>
public sealed record TopicDetailDto
{
    public long Id { get; init; }
    public long? DirectionId { get; init; }
    public int DepartmentId { get; init; }
    public int SupervisorId { get; init; }
    public int AcademicYearId { get; init; }
    public int WorkTypeId { get; init; }
    
    public string TitleRu { get; init; } = null!;
    public string? TitleEn { get; init; }
    public string? TitleKz { get; init; }
    public string? Description { get; init; }
    
    public int MaxParticipants { get; init; }
    public int AvailableSpots { get; init; }
    public bool IsApproved { get; init; }
    public bool IsClosed { get; init; }
    public bool IsTeamTopic { get; init; }
    
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
    
    /// <summary>
    /// List of applications for this topic.
    /// </summary>
    public IReadOnlyCollection<TopicApplicationDto>? Applications { get; init; }
}

/// <summary>
/// Data Transfer Object for TopicApplication entity.
/// </summary>
public sealed record TopicApplicationDto
{
    public long Id { get; init; }
    public int StudentId { get; init; }
    public string Status { get; init; } = null!;
    public DateTime AppliedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public int? ReviewedBy { get; init; }
    public string? ReviewComment { get; init; }
}