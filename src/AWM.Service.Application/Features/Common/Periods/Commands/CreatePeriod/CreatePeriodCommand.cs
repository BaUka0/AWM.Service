namespace AWM.Service.Application.Features.Common.Stages.Commands.CreateStage;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record CreateStageCommand : IRequest<Result<int>>
{
    public int DepartmentId { get; init; }
    public int SemesterId { get; init; }
    public int WorkflowStageId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
