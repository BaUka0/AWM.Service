namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.DeleteAttachment;

using AWM.Service.Domain.Thesis.Constants;
using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICheckTypeRepository _checkTypeRepository;
    private readonly IAttachmentTypeRepository _attachmentTypeRepository;
    private readonly ILogger<DeleteAttachmentCommandHandler> _logger;

    public DeleteAttachmentCommandHandler(
        IStudentWorkRepository workRepository,
        IAttachmentService attachmentService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ICheckTypeRepository checkTypeRepository,
        IAttachmentTypeRepository attachmentTypeRepository,
        ILogger<DeleteAttachmentCommandHandler> logger)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _checkTypeRepository = checkTypeRepository ?? throw new ArgumentNullException(nameof(checkTypeRepository));
        _attachmentTypeRepository = attachmentTypeRepository ?? throw new ArgumentNullException(nameof(attachmentTypeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User is not authenticated."));

            var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
            if (work is null)
                return Result.Failure(new Error("404", $"StudentWork with ID {request.WorkId} not found."));

            var attachment = work.Attachments.FirstOrDefault(a => a.Id == request.AttachmentId);
            if (attachment is null)
                return Result.Failure(new Error("404", $"Attachment with ID {request.AttachmentId} not found on this work."));

            var attachmentType = await _attachmentTypeRepository.GetByIdAsync(attachment.AttachmentTypeId, cancellationToken);
            if (attachmentType is null)
                return Result.Failure(new Error("404", $"AttachmentType with ID {attachment.AttachmentTypeId} not found."));

            // Block deleting work versions (Draft/Final) after NormControl is passed
            var normControlCheckType = await _checkTypeRepository.GetByCodeAsync(CheckTypeCodes.NormControl, cancellationToken);
            
            if (normControlCheckType is not null && 
                work.HasPassedCheck(normControlCheckType.Id) && 
                (attachmentType.Code == "DRAFT" || attachmentType.Code == "FINAL"))
            {
                return Result.Failure(new Error("BusinessRule.Attachment",
                    "Cannot delete work versions after NormControl has been passed."));
            }

            var storagePath = attachment.FileStoragePath;

            // Updates aggregate
            work.RemoveAttachment(request.AttachmentId, userId.Value);

            await _workRepository.UpdateAsync(work, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Remove record from the physical store *after* DB success
            try
            {
                await _attachmentService.DeleteAsync(storagePath, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete physical file at path '{StoragePath}' after DB record was removed.", storagePath);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while deleting the attachment: {ex.Message}"));
        }
    }
}
