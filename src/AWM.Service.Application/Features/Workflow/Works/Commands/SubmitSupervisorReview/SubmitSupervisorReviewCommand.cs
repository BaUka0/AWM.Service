using KDS.Primitives.FluentResult;
using MediatR;
using System.IO;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.SubmitSupervisorReview;

public sealed record SubmitSupervisorReviewCommand(
    long WorkId,
    Stream FileStream,
    string FileName,
    long FileSizeBytes,
    string ContentType,
    string Comment) : IRequest<Result>;
