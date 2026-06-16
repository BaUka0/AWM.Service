using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.SubmitDirection;

public record SubmitDirectionCommand(
    long DirectionId) : IRequest<Result<MediatR.Unit>>;
