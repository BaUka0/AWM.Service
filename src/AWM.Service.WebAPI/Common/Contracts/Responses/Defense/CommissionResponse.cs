namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for commission data (list view).
/// </summary>
public sealed record CommissionResponse
{
    /// <summary>
    /// Commission ID.
    /// </summary>
    /// <example>1</example>
    public int Id { get; init; }

    /// <summary>
    /// Department ID.
    /// </summary>
    /// <example>1</example>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    /// <example>2</example>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Type of commission (PreDefense or GAK).
    /// </summary>
    /// <example>PreDefense</example>
    public string CommissionType { get; init; } = null!;

    /// <summary>
    /// Commission name.
    /// </summary>
    /// <example>Комиссия предзащиты №1</example>
    public string? Name { get; init; }

    /// <summary>
    /// Pre-defense round number (1, 2, or 3). Null for GAK commissions.
    /// </summary>
    /// <example>1</example>
    public int? PreDefenseNumber { get; init; }

    /// <summary>
    /// Total number of members in the commission.
    /// </summary>
    /// <example>5</example>
    public int MemberCount { get; init; }

    /// <summary>
    /// Date and time when the commission was created.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; init; }
}
