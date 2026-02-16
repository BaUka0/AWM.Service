namespace AWM.Service.Application.Features.Thesis.Topics.DTOs;

/// <summary>
/// Data Transfer Object for Topic entity (list view).
/// </summary>
public sealed record TopicDto
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
    
    public int MaxParticipants { get; init; }
    public int AvailableSpots { get; init; }
    public bool IsApproved { get; init; }
    public bool IsClosed { get; init; }
    public bool IsTeamTopic { get; init; }
    
    public DateTime CreatedAt { get; init; }
}