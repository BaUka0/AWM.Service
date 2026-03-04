namespace AWM.Service.Application.Features.Thesis.Works.Commands.AssignReviewerToWork;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record AssignReviewerToWorkCommand : IRequest<Result<long>>
{
    public long WorkId { get; init; }
    public int ReviewerId { get; init; }
}
