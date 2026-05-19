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
    /// Org unit ID.
    /// </summary>
    public int OrgUnitId { get; init; }

    /// <summary>
    /// Semester ID.
    /// </summary>
    public int SemesterId { get; init; }

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
    /// Name of the commission chairman.
    /// </summary>
    public string? ChairmanName { get; init; }
    
    /// <summary>
    /// Name of the commission secretary.
    /// </summary>
    public string? SecretaryName { get; init; }

    /// <summary>
    /// Date and time when the commission was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
