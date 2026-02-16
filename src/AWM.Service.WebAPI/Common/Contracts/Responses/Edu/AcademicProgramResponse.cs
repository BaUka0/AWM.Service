namespace AWM.Service.WebAPI.Common.Contracts.Responses.Edu;

/// <summary>
/// Response contract for academic program.
/// </summary>
public sealed record AcademicProgramResponse
{
    public int Id { get; init; }
    public int DepartmentId { get; init; }
    public int DegreeLevelId { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }

    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }

    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
    public int? DeletedBy { get; init; }
}