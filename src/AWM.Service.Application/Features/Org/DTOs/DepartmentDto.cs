namespace AWM.Service.Application.Features.Org.DTOs;

/// <summary>
/// Data Transfer Object for Department entity.
/// </summary>
public sealed record DepartmentDto
{
    public int Id { get; init; }
    public int InstituteId { get; init; }
    public string Name { get; init; } = null!;
    public string? Code { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}