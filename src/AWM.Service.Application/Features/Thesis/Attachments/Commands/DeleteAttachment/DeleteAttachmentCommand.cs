namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.DeleteAttachment;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record DeleteAttachmentCommand : IRequest<Result>
{
    public long AttachmentId { get; init; }
    public long WorkId { get; init; }
}
