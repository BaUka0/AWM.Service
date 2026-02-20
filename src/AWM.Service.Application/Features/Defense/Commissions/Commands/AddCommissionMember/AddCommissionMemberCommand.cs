namespace AWM.Service.Application.Features.Defense.Commissions.Commands.AddCommissionMember;

using AWM.Service.Domain.Defense.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to add a member to a commission.
/// </summary>
public sealed record AddCommissionMemberCommand : IRequest<Result<int>>
{
    /// <summary>
    /// Commission ID to add the member to.
    /// </summary>
    public int CommissionId { get; init; }

    /// <summary>
    /// User ID of the member to add.
    /// </summary>
    public int UserId { get; init; }

    /// <summary>
    /// Role of the member in the commission.
    /// </summary>
    public RoleInCommission RoleInCommission { get; init; }
}
