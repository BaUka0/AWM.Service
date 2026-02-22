namespace AWM.Service.Application.Features.Thesis.Works.DTOs;

using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// DTO for student work data (list view).
/// </summary>
public sealed record StudentWorkDto
{
    public long Id { get; init; }
    public long? TopicId { get; init; }
    public int AcademicYearId { get; init; }
    public int DepartmentId { get; init; }
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
            AcademicYearId = entity.AcademicYearId,
            DepartmentId = entity.DepartmentId,
            CurrentStateId = entity.CurrentStateId,
            IsDefended = entity.IsDefended,
            FinalGrade = entity.FinalGrade,
            CreatedAt = entity.CreatedAt
        };
    }
}
