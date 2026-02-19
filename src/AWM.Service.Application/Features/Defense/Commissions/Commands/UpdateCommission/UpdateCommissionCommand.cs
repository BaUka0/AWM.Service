namespace AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update a commission's name.
/// </summary>
public sealed record UpdateCommissionCommand : IRequest<Result>
{
    /// <summary>
    /// Commission ID to update.
    /// </summary>
    public int CommissionId { get; init; }

    /// <summary>
    /// New name for the commission.
    /// </summary>
    public string Name { get; init; } = null!;
}
