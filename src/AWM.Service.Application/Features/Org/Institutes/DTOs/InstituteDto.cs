namespace AWM.Service.Application.Features.Org.Institutes.DTOs;

using AWM.Service.Application.Features.Org.Departments.DTOs;

/// <summary>
/// Data Transfer Object for Institute entity.
/// </summary>
public sealed record InstituteDto
{
    public int Id { get; init; }
    public int UniversityId { get; init; }
    public string Name { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }

    /// <summary>
    /// Collection of departments belonging to this institute (optional, for detailed views).
    /// </summary>
    public IReadOnlyCollection<DepartmentDto>? Departments { get; init; }
}