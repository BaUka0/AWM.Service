namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionById;

using AWM.Service.Application.Features.Defense.Commissions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve a specific commission by ID with full details including members.
/// </summary>
public sealed record GetCommissionByIdQuery : IRequest<Result<CommissionDetailDto>>
{
    /// <summary>
    /// Commission ID to retrieve.
    /// </summary>
    public int CommissionId { get; init; }
}
