using AWM.Service.Application.Features.Workflow.Attachments.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Attachments.Queries.GetWorkAttachments;

public record GetWorkAttachmentsQuery(long WorkId) : IRequest<Result<IReadOnlyList<AttachmentDto>>>;
