using AWM.Service.Application.Features.Workflow.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetWorkHistory;

public sealed record GetWorkHistoryQuery(long WorkId) : IRequest<Result<IReadOnlyList<WorkHistoryDto>>>;
