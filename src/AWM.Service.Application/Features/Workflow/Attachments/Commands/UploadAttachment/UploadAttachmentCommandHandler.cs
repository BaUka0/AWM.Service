using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Service;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, Result<long>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IAttachmentTypeRepository _attachmentTypeRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IAttachmentService _attachmentService;
    private readonly StorageSettings _storageSettings;
    private readonly IUnitOfWork _unitOfWork;

    public UploadAttachmentCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        IAttachmentTypeRepository attachmentTypeRepository,
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IAttachmentService attachmentService,
        IOptions<StorageSettings> storageSettingsOptions,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _attachmentTypeRepository = attachmentTypeRepository;
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _attachmentService = attachmentService;
        _storageSettings = storageSettingsOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<long>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<long>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        // Verify permissions (participant or supervisor)
        var isParticipant = work.Participants.Any(p => p.StudentId == currentUserId);
        var isSupervisor = false;
        if (work.TopicId.HasValue)
        {
            var topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
            isSupervisor = topic != null && topic.CreatedBy == currentUserId;
        }

        if (!isParticipant && !isSupervisor)
        {
            return Result.Failure<long>(new Error("Attachments.Forbidden", "You do not have permission to upload attachments for this work."));
        }

        // Verify attachment type
        var attachmentType = await _attachmentTypeRepository.GetByIdAsync(request.AttachmentTypeId, cancellationToken);
        if (attachmentType == null)
        {
            return Result.Failure<long>(new Error("AttachmentTypes.NotFound", $"Attachment type with ID {request.AttachmentTypeId} not found."));
        }

        // Validate size
        long maxSizeBytes = _storageSettings.MaxAttachmentSizeMb * 1024L * 1024L;
        if (request.FileSizeBytes > maxSizeBytes)
        {
            return Result.Failure<long>(new Error("Attachments.FileTooLarge", $"File size exceeds limit of {_storageSettings.MaxAttachmentSizeMb} MB."));
        }

        // Compute file hash
        var hash = await _attachmentService.ComputeHashAsync(request.FileStream, cancellationToken);

        // Reset stream position (just in case)
        if (request.FileStream.CanSeek)
        {
            request.FileStream.Position = 0;
        }

        // Save file physically
        var storagePath = await _attachmentService.SaveAsync(request.FileName, request.FileStream, request.ContentType, cancellationToken);

        // Add attachment in Domain
        var attachment = work.AddAttachment(
            request.AttachmentTypeId,
            request.FileName,
            storagePath,
            hash,
            currentUserId,
            request.FileSizeBytes,
            request.ContentType
        );

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(attachment.Id);
    }
}
