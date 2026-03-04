namespace AWM.Service.Application.Features.Workflow.Queries.GetAllWorkTypes;

using AWM.Service.Application.Features.Workflow.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetAllWorkTypesQuery.
/// Returns all work types from the database for frontend dropdowns.
/// </summary>
public sealed class GetAllWorkTypesQueryHandler
    : IRequestHandler<GetAllWorkTypesQuery, Result<IReadOnlyList<WorkTypeDto>>>
{
    private readonly IWorkflowRepository _workflowRepository;

    public GetAllWorkTypesQueryHandler(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<Result<IReadOnlyList<WorkTypeDto>>> Handle(
        GetAllWorkTypesQuery request,
        CancellationToken cancellationToken)
    {
        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);

        var dtos = workTypes.Select(w => new WorkTypeDto
        {
            Id = w.Id,
            Name = w.Name,
            DegreeLevelId = w.DegreeLevelId
        }).ToList();

        return Result.Success<IReadOnlyList<WorkTypeDto>>(dtos);
    }
}
