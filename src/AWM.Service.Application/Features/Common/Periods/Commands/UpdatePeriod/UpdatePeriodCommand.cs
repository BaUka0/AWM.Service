namespace AWM.Service.Application.Features.Common.Stages.Commands.UpdateStage;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record UpdateStageCommand : IRequest<Result>
{
    public int StageId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsActive { get; init; }
}
