namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.UploadReview;

using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO;

public sealed class UploadReviewCommandHandler : IRequestHandler<UploadReviewCommand, Result>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UploadReviewCommandHandler(
        IReviewRepository reviewRepository,
        IAttachmentService attachmentService,
        ICurrentUserProvider currentUserProvider)
    {
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UploadReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
            return Result.Failure(new Error("401", "User is not authenticated."));

        var existingReview = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (existingReview is null)
            return Result.Failure(new Error("404", $"Review with ID {request.ReviewId} not found."));

        // Verify that the current user is the assigned reviewer (in a real system)
        // or has admin rights. For now we assume implicitly trusted by endpoint permission.

        string? storagePath = existingReview.FileStoragePath;

        if (request.File is not null)
        {
            // Delete old file if updating
            if (!string.IsNullOrWhiteSpace(storagePath))
            {
                try
                {
                    await _attachmentService.DeleteAsync(storagePath, cancellationToken);
                }
                catch
                {
                    // Ignore deletion error
                }
            }

            await using var uploadStream = request.File.OpenReadStream();
            storagePath = await _attachmentService.SaveAsync(
                request.File.FileName,
                uploadStream,
                request.File.ContentType,
                cancellationToken);
        }

        try
        {
            existingReview.UploadReview(
                request.ReviewText ?? existingReview.ReviewText,
                storagePath,
                userId.Value);

            await _reviewRepository.UpdateAsync(existingReview, cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("400", argEx.Message));
        }
    }
}
