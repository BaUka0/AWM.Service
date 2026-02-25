namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.DeleteAttachment;

using AWM.Service.Domain.Thesis.Service;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAttachmentCommandHandler(
        IStudentWorkRepository workRepository,
        IAttachmentService attachmentService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

            var storagePath = attachment.FileStoragePath;

            // Updates aggregate
            work.RemoveAttachment(request.AttachmentId, userId.Value);

            await _workRepository.UpdateAsync(work, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Remove record from the physical store *after* DB success
            await _attachmentService.DeleteAsync(storagePath, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while deleting the attachment: {ex.Message}"));
        }
    }
}
