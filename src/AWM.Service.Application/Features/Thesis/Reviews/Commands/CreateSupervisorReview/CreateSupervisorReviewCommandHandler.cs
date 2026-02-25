namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.CreateSupervisorReview;

using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO;

public sealed class CreateSupervisorReviewCommandHandler : IRequestHandler<CreateSupervisorReviewCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly ISupervisorReviewRepository _reviewRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateSupervisorReviewCommandHandler(
        IStudentWorkRepository workRepository,
        ISupervisorReviewRepository reviewRepository,
        IAttachmentService attachmentService,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(CreateSupervisorReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
            return Result.Failure<long>(new Error("401", "User is not authenticated."));

        var work = await _workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<long>(new Error("404", $"StudentWork with ID {request.WorkId} not found."));

        string? storagePath = null;

        var existingReview = await _reviewRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);

        if (request.File is not null)
        {
            await using var uploadStream = request.File.OpenReadStream();
            storagePath = await _attachmentService.SaveAsync(
                request.File.FileName,
                uploadStream,
                request.File.ContentType,
                cancellationToken);
        }

        if (existingReview is not null)
        {
            // Update existing review
            if (request.File is not null && !string.IsNullOrWhiteSpace(existingReview.FileStoragePath))
            {
                // Optionally delete old file to save space
                try
                {
                    await _attachmentService.DeleteAsync(existingReview.FileStoragePath, cancellationToken);
                }
                catch
                {
                    // Ignore deletion error (e.g. file missing)
                }
            }
            else if (request.File is null && existingReview.FileStoragePath is not null)
            {
                // Keep the old file if no new one provided
                storagePath = existingReview.FileStoragePath;
            }

            existingReview.UpdateReview(request.ReviewText, storagePath, userId.Value);
            await _reviewRepository.UpdateAsync(existingReview, cancellationToken);

            return Result.Success(existingReview.Id);
        }
        else
        {
            // The supervisor logic: in real system we would verify that userId is actually the supervisor
            // Assuming for now userId = supervisorId or it's implicitly trusted via permission
            var supervisorId = userId.Value; // Ideally retrieved from work/topic details

            var review = new SupervisorReview(
                work.Id,
                supervisorId,
                request.ReviewText,
                userId.Value,
                storagePath);

            await _reviewRepository.AddAsync(review, cancellationToken);

            return Result.Success(review.Id);
        }
    }
}
