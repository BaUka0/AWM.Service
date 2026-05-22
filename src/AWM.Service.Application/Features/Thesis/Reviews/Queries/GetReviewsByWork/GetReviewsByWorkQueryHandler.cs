namespace AWM.Service.Application.Features.Thesis.Reviews.Queries.GetReviewsByWork;

using AWM.Service.Application.Features.Thesis.Reviews.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;

public sealed class GetReviewsByWorkQueryHandler : IRequestHandler<GetReviewsByWorkQuery, Result<WorkReviewsDto>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IWorkReviewRepository _workReviewRepository;

    public GetReviewsByWorkQueryHandler(
        IStudentWorkRepository workRepository,
        IWorkReviewRepository workReviewRepository)
    {
        _workRepository = workRepository;
        _workReviewRepository = workReviewRepository;
    }

    public async Task<Result<WorkReviewsDto>> Handle(GetReviewsByWorkQuery request, CancellationToken cancellationToken)
    {
        var work = await _workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<WorkReviewsDto>(new Error("404", $"StudentWork with ID {request.WorkId} not found."));

        var reviews = await _workReviewRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);

        var dto = new WorkReviewsDto
        {
            Reviews = reviews.Select(r => new WorkReviewDto
            {
                Id = r.Id,
                WorkId = r.WorkId,
                AuthorUserId = r.AuthorUserId,
                Type = r.Type,
                ReviewText = r.ReviewText,
                MetadataJson = r.MetadataJson,
                IsFinal = r.IsFinal,
                CreatedAt = r.CreatedAt,
                CreatedBy = r.CreatedBy,
                LastModifiedAt = r.LastModifiedAt,
                LastModifiedBy = r.LastModifiedBy
            }).ToList()
        };

        return Result.Success(dto);
    }
}
