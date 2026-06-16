using AWM.Service.Application.Features.Workflow.Attachments.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Attachments.Queries.DownloadAttachment;

public record DownloadAttachmentQuery(long WorkId, long AttachmentId) : IRequest<Result<FileDownloadDto>>;
