using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.DeleteAttachment;

public record DeleteAttachmentCommand(long WorkId, long AttachmentId) : IRequest<Result>;
