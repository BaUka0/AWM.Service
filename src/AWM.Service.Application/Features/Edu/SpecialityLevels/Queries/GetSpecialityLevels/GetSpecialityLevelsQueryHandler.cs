namespace AWM.Service.Application.Features.Edu.SpecialityLevels.Queries.GetSpecialityLevels;

using AWM.Service.Application.Features.Edu.SpecialityLevels.DTOs;
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
public sealed class GetSpecialityLevelsQueryHandler 
    : IRequestHandler<GetSpecialityLevelsQuery, Result<IReadOnlyList<SpecialityLevelDto>>>
{
    private readonly ISpecialityLevelRepository _SpecialityLevelRepository;

    public GetSpecialityLevelsQueryHandler(
        ISpecialityLevelRepository SpecialityLevelRepository)
    {
        _SpecialityLevelRepository = SpecialityLevelRepository;
    }

    public async Task<Result<IReadOnlyList<SpecialityLevelDto>>> Handle(
        GetSpecialityLevelsQuery request, 
        CancellationToken cancellationToken)
    {
        var levels = await _SpecialityLevelRepository.GetAllAsync(cancellationToken);
        
        var queryable = levels.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            queryable = queryable.Where(l => l.Title.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
        }

        var dtos = queryable.Select(l => new SpecialityLevelDto
        {
            Id = l.Id,
            Name = l.Title,
            DurationYears = 0
        }).ToList();

        return Result.Success<IReadOnlyList<SpecialityLevelDto>>(dtos);
    }
}
