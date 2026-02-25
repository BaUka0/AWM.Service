namespace AWM.Service.Application.Features.Thesis.Reviews.Queries.GetReviewsByWork;

using AWM.Service.Application.Features.Thesis.Reviews.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetReviewsByWorkQueryHandler : IRequestHandler<GetReviewsByWorkQuery, Result<WorkReviewsDto>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly ISupervisorReviewRepository _supervisorReviewRepository;
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByWorkQueryHandler(
        IStudentWorkRepository workRepository,
        ISupervisorReviewRepository supervisorReviewRepository,
        IReviewRepository reviewRepository)
    {
        _workRepository = workRepository;
        _supervisorReviewRepository = supervisorReviewRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<WorkReviewsDto>> Handle(GetReviewsByWorkQuery request, CancellationToken cancellationToken)
    {
        var work = await _workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<WorkReviewsDto>(new Error("404", $"StudentWork with ID {request.WorkId} not found."));

        var supervisorReview = await _supervisorReviewRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
        var reviews = await _reviewRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);

        // Map Supervisor Review
        SupervisorReviewDto? supervisorDto = null;
        if (supervisorReview is not null)
        {
            supervisorDto = new SupervisorReviewDto
            {
                Id = supervisorReview.Id,
                WorkId = supervisorReview.WorkId,
                SupervisorId = supervisorReview.SupervisorId,
                ReviewText = supervisorReview.ReviewText,
                FileStoragePath = supervisorReview.FileStoragePath,
                CreatedAt = supervisorReview.CreatedAt,
                CreatedBy = supervisorReview.CreatedBy,
                LastModifiedAt = supervisorReview.LastModifiedAt,
                LastModifiedBy = supervisorReview.LastModifiedBy
            };
        }

        // Map External Reviews
        var reviewDtos = new List<ReviewDto>();
        if (reviews is not null)
        {
            // Note: If GetByWorkIdAsync returns a single review or list (usually work has 1 or more external reviews)
            // Let's assume GetByWorkIdAsync returns a single Review for now based on typical interface,
            // or IReadOnlyList. I will handle it as a single review to be safe, then wrap in list.
            
            // Checking if `reviews` is IEnumerable or single object
            if (reviews is IEnumerable<AWM.Service.Domain.Thesis.Entities.Review> list)
            {
                foreach (var r in list)
                {
                    reviewDtos.Add(MapReviewToDto(r));
                }
            }
            else
            {
                reviewDtos.Add(MapReviewToDto((AWM.Service.Domain.Thesis.Entities.Review)(object)reviews));
            }
        }

        var dto = new WorkReviewsDto
        {
            SupervisorReview = supervisorDto,
            Reviews = reviewDtos
        };

        return Result.Success(dto);
    }

    private static ReviewDto MapReviewToDto(AWM.Service.Domain.Thesis.Entities.Review r)
    {
        return new ReviewDto
        {
            Id = r.Id,
            WorkId = r.WorkId,
            ReviewerId = r.ReviewerId,
            ReviewText = r.ReviewText,
            FileStoragePath = r.FileStoragePath,
            IsUploaded = r.IsUploaded,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy,
            LastModifiedAt = r.LastModifiedAt,
            LastModifiedBy = r.LastModifiedBy
        };
    }
}
