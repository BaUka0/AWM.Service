using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.UpdateCommission;

public sealed record UpdateCommissionCommand(
    int Id,
    string? Name) : IRequest<Result>;
