namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetReviewStatusByDepartment;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetReviewStatusByDepartmentQueryHandler
    : IRequestHandler<GetReviewStatusByDepartmentQuery, Result<ReviewStatusByDepartmentDto>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetReviewStatusByDepartmentQueryHandler(
        IStudentWorkRepository workRepository,
        IReviewRepository reviewRepository,
        IReviewerRepository reviewerRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _reviewerRepository = reviewerRepository ?? throw new ArgumentNullException(nameof(reviewerRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<ReviewStatusByDepartmentDto>> Handle(
        GetReviewStatusByDepartmentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<ReviewStatusByDepartmentDto>(new Error("401", "User ID is not available."));

            var works = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);

            var items = new List<WorkReviewStatusItem>();

            // Bulk fetch all reviews for works in this department (avoids N+1)
            var workIds = works.Select(w => w.Id).ToList();
            var allReviews = await _reviewRepository.GetByWorkIdsAsync(workIds, cancellationToken);
            var reviewsByWorkId = allReviews.GroupBy(r => r.WorkId)
                .ToDictionary(g => g.Key, g => g.First());

            // Bulk fetch all relevant reviewers
            var reviewerIds = reviewsByWorkId.Values
                .Select(r => r.ReviewerId)
                .Distinct()
                .ToList();
            var allReviewers = await _reviewerRepository.GetByIdsAsync(reviewerIds, cancellationToken);
            var reviewersById = allReviewers.ToDictionary(r => r.Id);

            foreach (var work in works)
            {
                reviewsByWorkId.TryGetValue(work.Id, out var review);

                string? reviewerName = null;
                if (review is not null && reviewersById.TryGetValue(review.ReviewerId, out var reviewer))
                {
                    reviewerName = reviewer.FullName;
                }

                items.Add(new WorkReviewStatusItem
                {
                    WorkId = work.Id,
                    ReviewerId = review?.ReviewerId,
                    ReviewerName = reviewerName,
                    HasReviewer = review is not null,
                    IsReviewUploaded = review?.IsUploaded ?? false
                });
            }

            var dto = new ReviewStatusByDepartmentDto
            {
                TotalWorks = items.Count,
                WorksWithReviewer = items.Count(i => i.HasReviewer),
                WorksWithoutReviewer = items.Count(i => !i.HasReviewer),
                ReviewsUploaded = items.Count(i => i.IsReviewUploaded),
                ReviewsPending = items.Count(i => i.HasReviewer && !i.IsReviewUploaded),
                Items = items
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ReviewStatusByDepartmentDto>(new Error("500", ex.Message));
        }
    }
}
