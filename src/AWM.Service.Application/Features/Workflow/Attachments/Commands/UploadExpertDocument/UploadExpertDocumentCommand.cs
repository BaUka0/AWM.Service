using KDS.Primitives.FluentResult;
using MediatR;
using System.IO;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadExpertDocument;

public record UploadExpertDocumentCommand(
    long WorkId,
    long QualityCheckId,
    int AttachmentTypeId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream) : IRequest<Result<long>>;
