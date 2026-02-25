namespace AWM.Service.Application.Features.Thesis.Attachments.Queries.GetAttachmentById;

using AWM.Service.Application.Features.Thesis.Attachments.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetAttachmentByIdQuery : IRequest<Result<AttachmentDto>>
{
    public long AttachmentId { get; init; }
    public long WorkId { get; init; }
}
