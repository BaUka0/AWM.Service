namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

using AWM.Service.Domain.Defense.Enums;

/// <summary>
/// Request contract for creating a defense commission.
/// </summary>
public sealed record CreateCommissionRequest
{
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
    /// Type of commission (0 = PreDefense, 1 = GAK).
    /// </summary>
    /// <example>0</example>
    public CommissionType CommissionType { get; init; }

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
}
