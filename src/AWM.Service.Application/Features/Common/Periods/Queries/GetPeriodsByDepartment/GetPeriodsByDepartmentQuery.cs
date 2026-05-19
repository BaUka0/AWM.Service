namespace AWM.Service.Application.Features.Common.Stages.Queries.GetStagesByDepartment;

using AWM.Service.Application.Features.Common.Stages.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStagesByDepartmentQuery : IRequest<Result<IReadOnlyList<StageDto>>>
{
    public int OrgUnitId { get; init; }
    public int SemesterId { get; init; }
}
