namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetAssignedReviewer;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetAssignedReviewerQueryHandler
    : IRequestHandler<GetAssignedReviewerQuery, Result<AssignedReviewerDto?>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetAssignedReviewerQueryHandler(
        IReviewRepository reviewRepository,
        IReviewerRepository reviewerRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _reviewerRepository = reviewerRepository ?? throw new ArgumentNullException(nameof(reviewerRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<AssignedReviewerDto?>> Handle(
        GetAssignedReviewerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<AssignedReviewerDto?>(new Error("401", "User ID is not available."));

            var reviews = await _reviewRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
            var review = reviews.FirstOrDefault();

            if (review is null)
                return Result.Success<AssignedReviewerDto?>(null);

            var reviewer = await _reviewerRepository.GetByIdAsync(review.ReviewerId, cancellationToken);
            if (reviewer is null)
                return Result.Success<AssignedReviewerDto?>(null);

            var dto = new AssignedReviewerDto
            {
                ReviewId = review.Id,
                ReviewerId = reviewer.Id,
                FullName = reviewer.FullName,
                Position = reviewer.Position,
                Organization = reviewer.Organization,
                Email = reviewer.Email,
                IsUploaded = review.IsUploaded
            };

            return Result.Success<AssignedReviewerDto?>(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AssignedReviewerDto?>(new Error("500", ex.Message));
        }
    }
}
