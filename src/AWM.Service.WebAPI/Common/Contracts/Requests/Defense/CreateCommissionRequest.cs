namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for creating a defense commission.
/// </summary>
public sealed record CreateCommissionRequest
{
    /// <summary>
    /// Org unit ID.
    /// </summary>
    /// <example>1</example>
    public int OrgUnitId { get; init; }

    /// <summary>
    /// Semester ID.
    /// </summary>
    /// <example>2</example>
    public int SemesterId { get; init; }

    /// <summary>
    /// Type of commission (1 = PreDefense, 2 = GAK).
    /// </summary>
    /// <example>1</example>
    public int CommissionType { get; init; }

    /// <summary>
    /// Custom name for the commission (optional — auto-generated if omitted).
    /// </summary>
    /// <example>Комиссия предзащиты №1</example>
    public string? Name { get; init; }

    /// <summary>
    /// Pre-defense round number (1, 2, or 3). Required for PreDefense type.
    /// </summary>
    /// <example>1</example>
    public int? PreDefenseNumber { get; init; }

    /// <summary>
    /// Initial members to add to the commission.
    /// </summary>
    public IReadOnlyList<CreateCommissionMemberRequest> Members { get; init; } = new List<CreateCommissionMemberRequest>();
}

public record CreateCommissionMemberRequest
{
    public int UserId { get; init; }
    public int Role { get; init; }
}
