namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for commission data with full details including members.
/// </summary>
public sealed record CommissionDetailResponse
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
    /// Date and time when the commission was created.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// User ID who created the commission.
    /// </summary>
    /// <example>5</example>
    public int CreatedBy { get; init; }

    /// <summary>
    /// Date and time of last modification (nullable).
    /// </summary>
    /// <example>2024-02-01T14:20:00Z</example>
    public DateTime? LastModifiedAt { get; init; }

    /// <summary>
    /// User ID who last modified the commission (nullable).
    /// </summary>
    /// <example>5</example>
    public int? LastModifiedBy { get; init; }

    /// <summary>
    /// List of commission members.
    /// </summary>
    public IReadOnlyCollection<CommissionMemberResponse> Members { get; init; } = [];
}
