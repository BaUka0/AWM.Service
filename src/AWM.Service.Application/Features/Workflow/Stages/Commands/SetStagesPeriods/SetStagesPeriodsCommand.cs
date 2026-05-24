using AWM.Service.Application.Features.Workflow.Stages.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.SetStagesPeriods;

public sealed record SetStagesPeriodsCommand(
    int SemesterId,
    List<StagePeriodDto> Periods,
    int? OrgUnitId = null,
    int? SpecialityId = null
) : IRequest<Result<Unit>>;
