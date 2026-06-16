using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.ReviewDirection;

public enum ReviewDecision
{
    Approve = 1,
    Reject = 2,
    RequireRevision = 3
}

public record ReviewDirectionCommand(
    long DirectionId,
    ReviewDecision Decision,
    string? Comment) : IRequest<Result<MediatR.Unit>>;
