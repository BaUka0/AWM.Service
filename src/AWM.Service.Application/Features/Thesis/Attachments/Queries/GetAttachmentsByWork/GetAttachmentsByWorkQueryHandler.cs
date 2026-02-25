namespace AWM.Service.Application.Features.Thesis.Attachments.Queries.GetAttachmentsByWork;

using AWM.Service.Application.Features.Thesis.Attachments.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetAttachmentsByWorkQueryHandler
    : IRequestHandler<GetAttachmentsByWorkQuery, Result<IReadOnlyList<AttachmentDto>>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetAttachmentsByWorkQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
    }

    public async Task<Result<IReadOnlyList<AttachmentDto>>> Handle(
        GetAttachmentsByWorkQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
            if (work is null)
                return Result.Failure<IReadOnlyList<AttachmentDto>>(
                    new Error("404", $"StudentWork with ID {request.WorkId} not found."));

            var dtos = work.Attachments
                .Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    WorkId = a.WorkId,
                    StateId = a.StateId,
                    AttachmentType = a.AttachmentType,
                    FileName = a.FileName,
                    FileStoragePath = a.FileStoragePath,
                    CreatedAt = a.CreatedAt,
                    CreatedBy = a.CreatedBy,
                    LastModifiedAt = a.LastModifiedAt,
                    LastModifiedBy = a.LastModifiedBy
                })
                .ToList();

            return Result.Success<IReadOnlyList<AttachmentDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<AttachmentDto>>(
                new Error("500", $"An error occurred while retrieving attachments: {ex.Message}"));
        }
    }
}
