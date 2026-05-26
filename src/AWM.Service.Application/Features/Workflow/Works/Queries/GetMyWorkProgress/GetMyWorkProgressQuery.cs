using AWM.Service.Application.Features.Workflow.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetMyWorkProgress;

public sealed record GetMyWorkProgressQuery : IRequest<Result<WorkProgressDto>>;
