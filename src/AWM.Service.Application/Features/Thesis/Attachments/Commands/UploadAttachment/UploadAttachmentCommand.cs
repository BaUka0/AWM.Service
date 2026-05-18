namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.UploadAttachment;

using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.AspNetCore.Http;

public sealed record UploadAttachmentCommand : IRequest<Result<long>>
{
    public long WorkId { get; init; }
    public int AttachmentTypeId { get; init; }

    /// <summary>
    /// The uploaded file (from IFormFile in the controller).
    /// </summary>
    public IFormFile File { get; init; } = null!;
}
