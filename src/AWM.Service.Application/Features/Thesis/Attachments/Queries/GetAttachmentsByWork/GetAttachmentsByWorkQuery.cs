namespace AWM.Service.Application.Features.Thesis.Attachments.Queries.GetAttachmentsByWork;

using AWM.Service.Application.Features.Thesis.Attachments.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetAttachmentsByWorkQuery : IRequest<Result<IReadOnlyList<AttachmentDto>>>
{
    public long WorkId { get; init; }
}
