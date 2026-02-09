namespace AWM.Service.Application.Features.Edu.DTOs;

/// <summary>
/// DTO for DegreeLevel entity.
/// </summary>
public sealed class DegreeLevelDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DurationYears { get; init; }

    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}