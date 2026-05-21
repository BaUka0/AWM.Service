namespace AWM.Service.Application.Features.Defense.Commissions.Commands.RemoveCommissionMember;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to remove a member from a commission.
/// </summary>
public sealed record RemoveCommissionMemberCommand : IRequest<Result>
{
    /// <summary>
    /// Commission ID to remove the member from.
    /// </summary>
    public int CommissionId { get; init; }

    /// <summary>
    /// Assignment record ID to remove.
    /// </summary>
    public long AssignmentId { get; init; }
}
