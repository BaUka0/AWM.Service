namespace AWM.Service.Application.Features.Thesis.Works.Commands.AssignReviewerToWork;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class AssignReviewerToWorkCommandHandler
    : IRequestHandler<AssignReviewerToWorkCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignReviewerToWorkCommandHandler> _logger;

    public AssignReviewerToWorkCommandHandler(
        IStudentWorkRepository workRepository,
        IReviewerRepository reviewerRepository,
        IReviewRepository reviewRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<AssignReviewerToWorkCommandHandler> logger)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _reviewerRepository = reviewerRepository ?? throw new ArgumentNullException(nameof(reviewerRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<long>> Handle(AssignReviewerToWorkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<long>(new Error("401", "User ID is not available."));

            var work = await _workRepository.GetByIdAsync(request.WorkId, cancellationToken);
            if (work is null)
                return Result.Failure<long>(new Error("NotFound.Work",
                    $"StudentWork with ID {request.WorkId} not found."));

            var reviewer = await _reviewerRepository.GetByIdAsync(request.ReviewerId, cancellationToken);
            if (reviewer is null)
                return Result.Failure<long>(new Error("NotFound.Reviewer",
                    $"Reviewer with ID {request.ReviewerId} not found."));

            if (!reviewer.IsActive)
                return Result.Failure<long>(new Error("BusinessRule.Reviewer",
                    "The specified reviewer is not active."));

            // Check if reviewer is already assigned to this work
            var existingReviews = await _reviewRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
            if (existingReviews.Any(r => r.ReviewerId == request.ReviewerId))
                return Result.Failure<long>(new Error("409",
                    "This reviewer is already assigned to this work."));

            var review = new Review(request.WorkId, request.ReviewerId, userId.Value);
            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Assigned reviewer {ReviewerId} to work {WorkId}", request.ReviewerId, request.WorkId);
            return Result.Success(review.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AssignReviewerToWork failed for Work={WorkId}", request.WorkId);
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
