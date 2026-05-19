namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new defense commission.
/// </summary>
public sealed record CreateCommissionCommand : IRequest<Result<int>>
{
    /// <summary>
    /// Org unit ID the commission belongs to.
    /// </summary>
    public int OrgUnitId { get; init; }

    /// <summary>
    /// Semester ID for the commission.
    /// </summary>
    public int SemesterId { get; init; }

    /// <summary>
    /// Type of commission (PreDefense or GAK).
    /// </summary>
    public int CommissionTypeId { get; init; }

    /// <summary>
    /// Optional name for the commission.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Pre-defense round number (1, 2, or 3). Required for PreDefense type, null for GAK.
    /// </summary>
    public int? PreDefenseNumber { get; init; }

    /// <summary>
    /// Initial members to add to the commission.
    /// </summary>
    public IReadOnlyList<CreateCommissionMemberCommand> Members { get; init; } = new List<CreateCommissionMemberCommand>();
}

public record CreateCommissionMemberCommand(int UserId, int CommissionRoleId);
