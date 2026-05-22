namespace AWM.Service.Application.Features.Thesis.Works.DTOs;

using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// DTO for student work data (list view).
/// </summary>
public sealed record StudentWorkDto
{
    public long Id { get; init; }
    public long? TopicId { get; init; }
    public int SemesterId { get; init; }
    public int OrgUnitId { get; init; }
    public int? SpecialityId { get; init; }
    public int CurrentStateId { get; init; }
    public bool IsDefended { get; init; }
    public string? FinalGrade { get; init; }
    public DateTime CreatedAt { get; init; }

    public static StudentWorkDto FromEntity(StudentWork entity)
    {
        return new StudentWorkDto
        {
            Id = entity.Id,
            TopicId = entity.TopicId,
            SemesterId = entity.SemesterId,
            OrgUnitId = entity.OrgUnitId,
            SpecialityId = entity.SpecialityId,
            CurrentStateId = entity.CurrentStateId,
            IsDefended = entity.IsDefended,
            FinalGrade = entity.FinalGrade,
            CreatedAt = entity.CreatedAt
        };
    }
}
