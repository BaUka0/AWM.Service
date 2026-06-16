using AWM.Service.Application.Features.Workflow.Stages.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Stages.Queries.GetStagesPeriods;

public sealed record GetStagesPeriodsQuery(
    int SemesterId,
    int? OrgUnitId = null,
    int? SpecialityId = null
) : IRequest<Result<IReadOnlyList<StagePeriodDto>>>;
