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

    public GetDirectionsBySupervisorQueryHandler(
        IDirectionRepository directionRepository)
    {
        _directionRepository = directionRepository;
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

            // Map to DTOs
            var result = filtered
                .Select(d => new DirectionDto
                {
                    Id = d.Id,
                    DepartmentId = d.DepartmentId,
                    SupervisorId = d.SupervisorId,
                    AcademicYearId = d.AcademicYearId,
                    WorkTypeId = d.WorkTypeId,
                    TitleRu = d.TitleRu,
                    TitleKz = d.TitleKz,
                    TitleEn = d.TitleEn,
                    CurrentStateId = d.CurrentStateId,
                    SubmittedAt = d.SubmittedAt,
                    ReviewedAt = d.ReviewedAt,
                    ReviewedBy = d.ReviewedBy,
                    CreatedAt = d.CreatedAt,
                    IsDeleted = d.IsDeleted
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