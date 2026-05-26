using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Constants;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.SubmitSupervisorReview;

public sealed class SubmitSupervisorReviewCommandHandler : IRequestHandler<SubmitSupervisorReviewCommand, Result>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IAttachmentTypeRepository _attachmentTypeRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IAttachmentService _attachmentService;
    private readonly StorageSettings _storageSettings;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitSupervisorReviewCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        ITopicRepository topicRepository,
        IAttachmentTypeRepository attachmentTypeRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IAttachmentService attachmentService,
        IOptions<StorageSettings> storageSettingsOptions,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _topicRepository = topicRepository;
        _attachmentTypeRepository = attachmentTypeRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _attachmentService = attachmentService;
        _storageSettings = storageSettingsOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitSupervisorReviewCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
            return Result.Failure(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));

        // Verify that current user is the supervisor of this work
        if (!work.TopicId.HasValue)
            return Result.Failure(new Error("SupervisorReview.NoTopic", "Work has no associated topic."));

        var topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
        if (topic == null || topic.CreatedBy != currentUserId)
            return Result.Failure(new Error("SupervisorReview.Forbidden", "Only the assigned scientific supervisor can submit this review."));

        // Validate size
        long maxSizeBytes = _storageSettings.MaxAttachmentSizeMb * 1024L * 1024L;
        if (request.FileSizeBytes > maxSizeBytes)
            return Result.Failure(new Error("SupervisorReview.FileTooLarge", $"File size exceeds limit of {_storageSettings.MaxAttachmentSizeMb} MB."));

        // Compute file hash and reset position
        var hash = await _attachmentService.ComputeHashAsync(request.FileStream, cancellationToken);
        if (request.FileStream.CanSeek)
            request.FileStream.Position = 0;

        // Save physical file
        var storagePath = await _attachmentService.SaveAsync(request.FileName, request.FileStream, request.ContentType, cancellationToken);

        // Retrieve attachment type by code or ID (ID 6 for SUPERVISOR_REVIEW)
        var attachmentType = await _attachmentTypeRepository.GetByIdAsync(6, cancellationToken);
        if (attachmentType == null)
            return Result.Failure(new Error("AttachmentTypes.NotFound", "Supervisor review attachment type not found."));

        // Add attachment in Domain
        work.AddAttachment(
            attachmentType.Id,
            request.FileName,
            storagePath,
            hash,
            currentUserId,
            request.FileSizeBytes,
            request.ContentType
        );

        // Add review comment
        var reviewText = string.IsNullOrWhiteSpace(request.Comment) ? "Supervisor review uploaded" : request.Comment;
        work.AddReview(currentUserId, ReviewType.SupervisorReview, reviewText, currentUserId);

        // Automation Hook: Transition to ReviewsWaitingForReviewer if currently in ReviewsWaitingForSupervisor
        var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
        if (currentState != null && currentState.SystemName == WorkStates.ReviewsWaitingForSupervisor)
        {
            var targetState = await _workflowRepository.GetStateBySystemNameAsync(currentState.WorkTypeId, WorkStates.ReviewsWaitingForReviewer, cancellationToken);
            if (targetState != null)
            {
                work.ChangeState(targetState.Id, currentUserId, "Supervisor review uploaded. Transitioning to waiting for external review.");
            }
        }

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
