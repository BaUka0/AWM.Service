namespace AWM.Service.Application.Features.Edu.DegreeLevels.Queries.GetDegreeLevels;

using AWM.Service.Application.Features.Edu.DegreeLevels.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Handler for retrieving degree levels.
/// </summary>
public sealed class GetDegreeLevelsQueryHandler 
    : IRequestHandler<GetDegreeLevelsQuery, Result<IReadOnlyList<DegreeLevelDto>>>
{
    private readonly ISpecialityLevelRepository _SpecialityLevelRepository;

    public GetDegreeLevelsQueryHandler(
        ISpecialityLevelRepository SpecialityLevelRepository)
    {
        _SpecialityLevelRepository = SpecialityLevelRepository;
    }

    public async Task<Result<IReadOnlyList<DegreeLevelDto>>> Handle(
        GetDegreeLevelsQuery request, 
        CancellationToken cancellationToken)
    {
        var levels = await _SpecialityLevelRepository.GetAllAsync(cancellationToken);
        
        var queryable = levels.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            queryable = queryable.Where(l => l.Title.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
        }

        var dtos = queryable.Select(l => new DegreeLevelDto
        {
            Id = l.Id,
            Name = l.Title,
            DurationYears = 0
        }).ToList();

        return Result.Success<IReadOnlyList<DegreeLevelDto>>(dtos);
    }
}