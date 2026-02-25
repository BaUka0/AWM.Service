namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.UploadAttachment;

using AWM.Service.Application.Features.Thesis.Attachments.Services;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UploadAttachmentCommandHandler(
        IStudentWorkRepository workRepository,
        IAttachmentService attachmentService,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attachmentService = attachmentService ?? throw new ArgumentNullException(nameof(attachmentService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<long>(new Error("401", "User is not authenticated."));

            var work = await _workRepository.GetByIdAsync(request.WorkId, cancellationToken);
            if (work is null)
                return Result.Failure<long>(new Error("404", $"StudentWork with ID {request.WorkId} not found."));

            // Compute hash before uploading to detect duplicates
            await using var hashStream = request.File.OpenReadStream();
            var fileHash = await _attachmentService.ComputeHashAsync(hashStream, cancellationToken);

            // Upload to storage backend
            await using var uploadStream = request.File.OpenReadStream();
            var storagePath = await _attachmentService.SaveAsync(
                request.File.FileName,
                uploadStream,
                request.File.ContentType,
                cancellationToken);

            // Create domain attachment through the aggregate root
            var attachment = work.AddAttachment(
                request.AttachmentType,
                request.File.FileName,
                storagePath,
                fileHash,
                userId.Value);

            await _workRepository.UpdateAsync(work, cancellationToken);

            return Result.Success(attachment.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<long>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("500", $"An error occurred while uploading the attachment: {ex.Message}"));
        }
    }
}
