namespace AWM.Service.Application.Features.Defense.Commissions.DTOs;

/// <summary>
/// Data Transfer Object for Commission entity (list view).
/// </summary>
public sealed record CommissionDto
{
    /// <summary>
    /// Commission ID.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Department ID.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Type of commission (PreDefense or GAK).
    /// </summary>
    public string CommissionType { get; init; } = null!;

    /// <summary>
    /// Commission name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Pre-defense number (1, 2, or 3 for PreDefense type; null for GAK).
    /// </summary>
    public int? PreDefenseNumber { get; init; }

    /// <summary>
    /// Total number of members in the commission.
    /// </summary>
    public int MemberCount { get; init; }

    /// <summary>
    /// Date and time when the commission was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
