namespace AWM.Service.Application.Features.Common.Stages.Queries.GetActiveStage;

using AWM.Service.Application.Features.Common.Stages.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetActiveStageQuery : IRequest<Result<StageDto?>>
{
    public int DepartmentId { get; init; }
    public int SemesterId { get; init; }
    public int? WorkflowStageId { get; init; }
}
