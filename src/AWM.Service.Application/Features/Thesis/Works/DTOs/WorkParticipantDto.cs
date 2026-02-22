namespace AWM.Service.Application.Features.Thesis.Works.DTOs;

using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// DTO for work participant data.
/// </summary>
public sealed record WorkParticipantDto
{
    public long Id { get; init; }
    public long WorkId { get; init; }
    public int StudentId { get; init; }
    public string Role { get; init; } = null!;
    public bool IsLeader { get; init; }
    public DateTime JoinedAt { get; init; }

    public static WorkParticipantDto FromEntity(WorkParticipant entity)
    {
        return new WorkParticipantDto
        {
            Id = entity.Id,
            WorkId = entity.WorkId,
            StudentId = entity.StudentId,
            Role = entity.Role.ToString(),
            IsLeader = entity.IsLeader,
            JoinedAt = entity.JoinedAt
        };
    }
}
