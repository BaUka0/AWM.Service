namespace AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionsBySupervisor;

using AWM.Service.Application.Features.Thesis.Directions.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving directions by supervisor.
/// </summary>
public sealed class GetDirectionsBySupervisorQueryHandler 
    : IRequestHandler<GetDirectionsBySupervisorQuery, Result<IReadOnlyList<DirectionDto>>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetDirectionsBySupervisorQueryHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result<IReadOnlyList<DirectionDto>>> Handle(
        GetDirectionsBySupervisorQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Get directions from repository
            var directions = await _directionRepository
                .GetBySupervisorAsync(request.SupervisorId, request.AcademicYearId, cancellationToken);

            // Apply in-memory filters
            var filtered = directions.AsEnumerable();

            // Filter by IsDeleted
            if (!request.IncludeDeleted)
            {
                filtered = filtered.Where(d => !d.IsDeleted);
            }

            // Filter by WorkType
            if (request.WorkTypeId.HasValue)
            {
                filtered = filtered.Where(d => d.WorkTypeId == request.WorkTypeId.Value);
            }

            // Filter by State
            if (request.StateId.HasValue)
            {
                filtered = filtered.Where(d => d.CurrentStateId == request.StateId.Value);
            }

            var filteredList = filtered.ToList();
            var statesById = (await _workflowRepository.GetStatesByIdsAsync(
                    filteredList.Select(direction => direction.CurrentStateId).Distinct(),
                    cancellationToken))
                .ToDictionary(state => state.Id, state => (state.SystemName, state.DisplayName));

            var result = filteredList
                .Select(direction =>
                {
                    statesById.TryGetValue(direction.CurrentStateId, out var state);

                    return new DirectionDto
                    {
                        Id = direction.Id,
                        OrgUnitId = direction.OrgUnitId,
                        EmployeeId = direction.EmployeeId,
                        SemesterId = direction.SemesterId,
                        WorkTypeId = direction.WorkTypeId,
                        TitleRu = direction.TitleRu,
                        TitleKz = direction.TitleKz,
                        TitleEn = direction.TitleEn,
                        DescriptionRu = direction.DescriptionRu,
                        DescriptionKz = direction.DescriptionKz,
                        DescriptionEn = direction.DescriptionEn,
                        CurrentStateId = direction.CurrentStateId,
                        CurrentStateName = state.SystemName,
                        CurrentStateDisplayName = state.DisplayName,
                        SubmittedAt = direction.SubmittedAt,
                        ReviewedAt = direction.ReviewedAt,
                        ReviewedBy = direction.ReviewedBy,
                        ReviewComment = direction.ReviewComment,
                        CreatedAt = direction.CreatedAt,
                        IsDeleted = direction.IsDeleted
                    };
                })
                .OrderByDescending(direction => direction.CreatedAt)
                .ToList();

            return Result.Success<IReadOnlyList<DirectionDto>>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<DirectionDto>>(
                new Error("InternalError", ex.Message));
        }
    }
}
