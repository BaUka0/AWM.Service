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
            var statesById = new Dictionary<int, (string? SystemName, string? DisplayName)>();
            foreach (var stateId in filteredList.Select(d => d.CurrentStateId).Distinct())
            {
                var state = await _workflowRepository.GetStateByIdAsync(stateId, cancellationToken);
                statesById[stateId] = (state?.SystemName, state?.DisplayName);
            }

            // Map to DTOs
            var result = filteredList
                .Select(d =>
                {
                    statesById.TryGetValue(d.CurrentStateId, out var state);

                    return new DirectionDto
                    {
                        Id = d.Id,
                        DepartmentId = d.DepartmentId,
                        SupervisorId = d.SupervisorId,
                        AcademicYearId = d.AcademicYearId,
                        WorkTypeId = d.WorkTypeId,
                        TitleRu = d.TitleRu,
                        TitleKz = d.TitleKz,
                        TitleEn = d.TitleEn,
                        DescriptionRu = d.DescriptionRu,
                        DescriptionKz = d.DescriptionKz,
                        DescriptionEn = d.DescriptionEn,
                        CurrentStateId = d.CurrentStateId,
                        CurrentStateName = state.SystemName,
                        CurrentStateDisplayName = state.DisplayName,
                        SubmittedAt = d.SubmittedAt,
                        ReviewedAt = d.ReviewedAt,
                        ReviewedBy = d.ReviewedBy,
                        ReviewComment = d.ReviewComment,
                        CreatedAt = d.CreatedAt,
                        IsDeleted = d.IsDeleted
                    };
                })
                .OrderByDescending(d => d.CreatedAt) // Most recent first
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
