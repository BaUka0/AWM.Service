using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.ApprovePreDefensePeriods;

public sealed record ApprovePreDefensePeriodsCommand(
    int OrgUnitId,
    int SemesterId) : IRequest<Result>;
