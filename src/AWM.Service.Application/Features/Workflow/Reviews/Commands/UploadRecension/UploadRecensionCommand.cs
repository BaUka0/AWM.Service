using KDS.Primitives.FluentResult;
using MediatR;
using System.IO;

namespace AWM.Service.Application.Features.Workflow.Reviews.Commands.UploadRecension;

public record UploadRecensionCommand(
    long WorkId,
    int ReviewerUserId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream) : IRequest<Result>;
