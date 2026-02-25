namespace AWM.Service.Application.Features.Thesis.Attachments.Queries.GetAttachmentById;

using AWM.Service.Application.Features.Thesis.Attachments.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetAttachmentByIdQueryHandler
    : IRequestHandler<GetAttachmentByIdQuery, Result<AttachmentDto>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetAttachmentByIdQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
    }

    public async Task<Result<AttachmentDto>> Handle(
        GetAttachmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
            if (work is null)
                return Result.Failure<AttachmentDto>(
                    new Error("404", $"StudentWork with ID {request.WorkId} not found."));

            var attachment = work.Attachments.FirstOrDefault(a => a.Id == request.AttachmentId);
            if (attachment is null)
                return Result.Failure<AttachmentDto>(
                    new Error("404", $"Attachment with ID {request.AttachmentId} not found on this work."));

            var dto = new AttachmentDto
            {
                Id = attachment.Id,
                WorkId = attachment.WorkId,
                StateId = attachment.StateId,
                AttachmentType = attachment.AttachmentType,
                FileName = attachment.FileName,
                FileStoragePath = attachment.FileStoragePath,
                CreatedAt = attachment.CreatedAt,
                CreatedBy = attachment.CreatedBy,
                LastModifiedAt = attachment.LastModifiedAt,
                LastModifiedBy = attachment.LastModifiedBy
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<AttachmentDto>(
                new Error("500", $"An error occurred while retrieving the attachment: {ex.Message}"));
        }
    }
}
