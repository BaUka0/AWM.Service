namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;
using AWM.Service.Domain.Defense.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new defense commission.
/// </summary>
public sealed record CreateCommissionCommand : IRequest<Result<int>>
{
    /// <summary>
    /// Department ID the commission belongs to.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID for the commission.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Type of commission (PreDefense or GAK).
    /// </summary>
    public CommissionType CommissionType { get; init; }

    /// <summary>
    /// Optional name for the commission.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Pre-defense round number (1, 2, or 3). Required for PreDefense type, null for GAK.
    /// </summary>
    public int? PreDefenseNumber { get; init; }
}
