using AWM.Service.Application.Features.Defense.Commissions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissions;

public sealed record GetCommissionsQuery(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null) : IRequest<Result<IReadOnlyList<CommissionDto>>>;
