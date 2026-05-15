using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.DeleteCommission;

public sealed record DeleteCommissionCommand : IRequest<Result>
{
    public int CommissionId { get; init; }
}
