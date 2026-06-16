using KDS.Primitives.FluentResult;
using MediatR;
using System.IO;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadAttachment;

public record UploadAttachmentCommand(
    long WorkId,
    int AttachmentTypeId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream) : IRequest<Result<long>>;
