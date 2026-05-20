namespace AWM.Service.Application.Features.Org.OrgUnits.DTOs;

/// <summary>
/// Unified DTO for any organizational unit (institute, department, etc.).
/// </summary>
public sealed record OrgUnitDto
{
    public int Id { get; init; }
    public int? ParentId { get; init; }
    public string Name { get; init; } = null!;
    public string? Code { get; init; }
    public int TypeId { get; init; }
}
