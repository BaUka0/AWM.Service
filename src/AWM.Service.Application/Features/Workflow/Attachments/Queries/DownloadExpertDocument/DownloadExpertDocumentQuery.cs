using AWM.Service.Application.Features.Workflow.Attachments.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Attachments.Queries.DownloadExpertDocument;

public record DownloadExpertDocumentQuery(long WorkId, long QualityCheckId) : IRequest<Result<FileDownloadDto>>;
