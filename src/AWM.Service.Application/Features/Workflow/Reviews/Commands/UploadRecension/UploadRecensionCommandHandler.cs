using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Constants;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Thesis.Service;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Options;

namespace AWM.Service.Application.Features.Workflow.Reviews.Commands.UploadRecension;

public sealed class UploadRecensionCommandHandler : IRequestHandler<UploadRecensionCommand, Result>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IAttachmentTypeRepository _attachmentTypeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IAttachmentService _attachmentService;
    private readonly StorageSettings _storageSettings;
    private readonly IUnitOfWork _unitOfWork;

    public UploadRecensionCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        IAttachmentTypeRepository attachmentTypeRepository,
        ICurrentUserProvider currentUserProvider,
        IAttachmentService attachmentService,
        IOptions<StorageSettings> storageSettingsOptions,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _attachmentTypeRepository = attachmentTypeRepository;
        _currentUserProvider = currentUserProvider;
        _attachmentService = attachmentService;
        _storageSettings = storageSettingsOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UploadRecensionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        long maxSizeBytes = _storageSettings.MaxAttachmentSizeMb * 1024L * 1024L;
        if (request.FileSizeBytes > maxSizeBytes)
        {
            return Result.Failure(new Error("Checks.FileTooLarge", $"File size exceeds limit of {_storageSettings.MaxAttachmentSizeMb} MB."));
        }

        var hash = await _attachmentService.ComputeHashAsync(request.FileStream, cancellationToken);
        if (request.FileStream.CanSeek)
        {
            request.FileStream.Position = 0;
        }

        var storagePath = await _attachmentService.SaveAsync(request.FileName, request.FileStream, request.ContentType, cancellationToken);

        var attachmentType = await _attachmentTypeRepository.GetByCodeAsync(AttachmentTypeCodes.ReviewDocument, cancellationToken);
        if (attachmentType == null)
            return Result.Failure(new Error("AttachmentTypes.NotFound", "Review document attachment type not found."));

        work.AddAttachment(
            attachmentType.Id,
            request.FileName,
            storagePath,
            hash,
            currentUserId,
            request.FileSizeBytes,
            request.ContentType
        );

        work.AddReview(request.ReviewerUserId, ReviewType.ExternalReview, "Recension uploaded", currentUserId);

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
