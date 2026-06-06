using AWM.Service.Application.Features.Workflow.Works.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetWorkHistory;

public sealed class GetWorkHistoryQueryHandler : IRequestHandler<GetWorkHistoryQuery, Result<IReadOnlyList<WorkHistoryDto>>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetWorkHistoryQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        IWorkflowRepository workflowRepository)
    {
        _studentWorkRepository = studentWorkRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<IReadOnlyList<WorkHistoryDto>>> Handle(GetWorkHistoryQuery request, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<IReadOnlyList<WorkHistoryDto>>(new Error("Work.NotFound", "Work not found."));
        }

        var history = work.WorkflowHistory.OrderBy(h => h.CreatedAt).ToList();
        if (history.Count == 0)
        {
            return Result.Success<IReadOnlyList<WorkHistoryDto>>(new List<WorkHistoryDto>());
        }

        var stateIds = history
            .SelectMany(h => h.FromStateId.HasValue ? new[] { h.FromStateId.Value, h.ToStateId } : new[] { h.ToStateId })
            .Distinct()
            .ToList();

        var states = await _workflowRepository.GetStatesByIdsAsync(stateIds, cancellationToken);
        var stateMap = states.ToDictionary(s => s.Id);

        var result = history.Select(h =>
        {
            var fromName = h.FromStateId.HasValue && stateMap.TryGetValue(h.FromStateId.Value, out var fromState)
                ? fromState.DisplayName
                : null;
            var toName = (stateMap.TryGetValue(h.ToStateId, out var toState)
                ? toState.DisplayName
                : "Unknown") ?? "Unknown";

            return new WorkHistoryDto(
                h.Id,
                h.FromStateId,
                fromName,
                h.ToStateId,
                toName,
                h.CreatedAt,
                h.Comment
            );
        }).ToList();

        return Result.Success<IReadOnlyList<WorkHistoryDto>>(result);
    }
}
