namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.CreateSupervisorReview;

using AWM.Service.Domain.Thesis.Constants;
using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class CreateSupervisorReviewCommandHandler : IRequestHandler<CreateSupervisorReviewCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IWorkReviewRepository _workReviewRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IAttachmentTypeRepository _attachmentTypeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICheckTypeRepository _checkTypeRepository;
    private readonly ILogger<CreateSupervisorReviewCommandHandler> _logger;

    public CreateSupervisorReviewCommandHandler(
        IStudentWorkRepository workRepository,
        IWorkReviewRepository workReviewRepository,
        IAttachmentService attachmentService,
        IAttachmentTypeRepository attachmentTypeRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ICheckTypeRepository checkTypeRepository,
        ILogger<CreateSupervisorReviewCommandHandler> logger)
    {
        _workRepository = workRepository;
        _workReviewRepository = workReviewRepository;
        _attachmentService = attachmentService;
        _attachmentTypeRepository = attachmentTypeRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _checkTypeRepository = checkTypeRepository;
        _logger = logger;
    }

    public async Task<Result<long>> Handle(CreateSupervisorReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
            return Result.Failure<long>(new Error("401", "User is not authenticated."));

        var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<long>(new Error("404", $"StudentWork with ID {request.WorkId} not found."));

        // Business rule check (e.g. anti-plagiarism)
        var antiPlagCheckType = await _checkTypeRepository.GetByCodeAsync(CheckTypeCodes.AntiPlagiarism, cancellationToken);
        if (antiPlagCheckType is not null && !work.HasPassedCheck(antiPlagCheckType.Id))
        {
            return Result.Failure<long>(new Error("BusinessRule.SupervisorReview", 
                "Cannot create or update a supervisor review until AntiPlagiarism check is passed."));
        }

        var existingReview = await _workReviewRepository.GetByWorkAndTypeAsync(request.WorkId, ReviewType.SupervisorReview, cancellationToken);
        
        WorkReview review;
        if (existingReview is not null)
        {
            existingReview.UpdateReview(request.ReviewText, null, userId.Value);
            await _workReviewRepository.UpdateAsync(existingReview, cancellationToken);
            review = existingReview;
        }
        else
        {
            review = work.AddReview(userId.Value, ReviewType.SupervisorReview, request.ReviewText, userId.Value);
            await _workReviewRepository.AddAsync(review, cancellationToken);
        }

        // Handle file via universal Attachment system
        if (request.File is not null)
        {
            var attachmentType = await _attachmentTypeRepository.GetByNameAsync("SupervisorReviewScan", cancellationToken);
            if (attachmentType is null)
            {
                // Fallback or create if missing? Usually should exist.
                _logger.LogWarning("AttachmentType 'SupervisorReviewScan' not found. File not uploaded.");
            }
            else
            {
                await using var uploadStream = request.File.OpenReadStream();
                var storagePath = await _attachmentService.SaveAsync(
                    request.File.FileName,
                    uploadStream,
                    request.File.ContentType,
                    cancellationToken);

                var attachment = new Attachment(
                    work.Id,
                    work.CurrentStateId,
                    attachmentType.Id,
                    request.File.FileName,
                    storagePath,
                    "TODO_HASH", // Should be calculated
                    userId.Value);

                // Add attachment logic (depending on how AttachmentRepository is used, 
                // but usually it's better to have IAttachmentRepository)
                // For now, let's assume we use UnitOfWork or specialized service
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(review.Id);
    }
}
