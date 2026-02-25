namespace AWM.Service.Application.Features.Thesis.Reviews.Queries.GetReviewsByWork;

using AWM.Service.Application.Features.Thesis.Reviews.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetReviewsByWorkQuery : IRequest<Result<WorkReviewsDto>>
{
    public long WorkId { get; init; }
}
