namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.CreateSupervisorReview;

using AWM.Service.Domain.Thesis.Constants;
using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;

public sealed class CreateSupervisorReviewCommandHandler : IRequestHandler<CreateSupervisorReviewCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly ISupervisorReviewRepository _reviewRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICheckTypeRepository _checkTypeRepository;
    private readonly ILogger<CreateSupervisorReviewCommandHandler> _logger;

    public CreateSupervisorReviewCommandHandler(
        IStudentWorkRepository workRepository,
        ISupervisorReviewRepository reviewRepository,
        IAttachmentService attachmentService,
        IEmployeeRepository EmployeeRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ICheckTypeRepository checkTypeRepository,
        ILogger<CreateSupervisorReviewCommandHandler> logger)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
        _EmployeeRepository = EmployeeRepository ?? throw new ArgumentNullException(nameof(EmployeeRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _checkTypeRepository = checkTypeRepository ?? throw new ArgumentNullException(nameof(checkTypeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<long>> Handle(CreateSupervisorReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
            return Result.Failure<long>(new Error("401", "User is not authenticated."));

        // Resolve staff profile — SupervisorReview.SupervisorId is Staff.Id (FK to Edu.Staff), not Auth.Users.Id
        var currentStaff = await _EmployeeRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (currentStaff is null)
            return Result.Failure<long>(new Error("403", "User does not have a staff profile."));

        var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<long>(new Error("404", $"StudentWork with ID {request.WorkId} not found."));

        var antiPlagCheckType = await _checkTypeRepository.GetByCodeAsync(CheckTypeCodes.AntiPlagiarism, cancellationToken);
        if (antiPlagCheckType is not null && !work.HasPassedCheck(antiPlagCheckType.Id))
        {
            return Result.Failure<long>(new Error("BusinessRule.SupervisorReview", 
                "Cannot create or update a supervisor review until AntiPlagiarism check is passed."));
        }

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
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete old supervisor review physical file at path '{StoragePath}'.", existingReview.FileStoragePath);
                }
            }
            else if (request.File is null && existingReview.FileStoragePath is not null)
            {
                // Keep the old file if no new one provided
                storagePath = existingReview.FileStoragePath;
            }

            existingReview.UpdateReview(request.ReviewText, storagePath, userId.Value);
            await _reviewRepository.UpdateAsync(existingReview, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(existingReview.Id);
        }
        else
        {
            // supervisorId is Staff.Id (FK to Edu.Staff), correctly resolved from user's staff profile
            var review = new SupervisorReview(
                work.Id,
                currentStaff.Id,
                request.ReviewText,
                userId.Value,
                storagePath);

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(review.Id);
        }
    }
}
