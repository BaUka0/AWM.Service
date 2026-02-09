namespace AWM.Service.Application.Features.Edu.Queries.DegreeLevels.GetDegreeLevels;

using AWM.Service.Application.Features.Edu.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving degree levels.
/// </summary>
public sealed class GetDegreeLevelsQueryHandler 
    : IRequestHandler<GetDegreeLevelsQuery, Result<IReadOnlyList<DegreeLevelDto>>>
{
    private readonly IDegreeLevelRepository _degreeLevelRepository;

    public GetDegreeLevelsQueryHandler(
        IDegreeLevelRepository degreeLevelRepository)
    {
        _degreeLevelRepository = degreeLevelRepository;
    }

    public async Task<Result<IReadOnlyList<DegreeLevelDto>>> Handle(
        GetDegreeLevelsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Get all degree levels from repository
            var degreeLevels = await _degreeLevelRepository
                .GetAllAsync(cancellationToken);

            // Apply in-memory filters
            var filtered = degreeLevels.AsEnumerable();

            // Filter by Name (partial match, case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                filtered = filtered.Where(d => 
                    d.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by minimum duration
            if (request.MinDurationYears.HasValue)
            {
                filtered = filtered.Where(d => 
                    d.DurationYears >= request.MinDurationYears.Value);
            }

            // Filter by maximum duration
            if (request.MaxDurationYears.HasValue)
            {
                filtered = filtered.Where(d => 
                    d.DurationYears <= request.MaxDurationYears.Value);
            }

            // Map to DTOs
            var result = filtered
                .Select(d => new DegreeLevelDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    DurationYears = d.DurationYears,
                    CreatedAt = d.CreatedAt,
                    CreatedBy = d.CreatedBy,
                    LastModifiedAt = d.LastModifiedAt,
                    LastModifiedBy = d.LastModifiedBy
                })
                .OrderBy(d => d.DurationYears) // Sort by duration (Bachelor -> Master -> PhD)
                .ToList();

            return Result.Success<IReadOnlyList<DegreeLevelDto>>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<DegreeLevelDto>>(
                new Error("InternalError", ex.Message));
        }
    }
}