namespace AWM.Service.Application.Features.Edu.Specialities.Queries.GetSpecialities;

using AWM.Service.Application.Features.Edu.Specialities.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Handler for retrieving academic programs with filtering.
/// </summary>
public sealed class GetSpecialitiesQueryHandler 
    : IRequestHandler<GetSpecialitiesQuery, Result<IReadOnlyList<SpecialityDto>>>
{
    private readonly ISpecialityRepository _SpecialityRepository;

    public GetSpecialitiesQueryHandler(
        ISpecialityRepository SpecialityRepository)
    {
        _SpecialityRepository = SpecialityRepository;
    }

    public async Task<Result<IReadOnlyList<SpecialityDto>>> Handle(
        GetSpecialitiesQuery request, 
        CancellationToken cancellationToken)
    {
        var specialities = await _SpecialityRepository.GetAllAsync(cancellationToken);
        
        var queryable = specialities.AsEnumerable();

        if (request.DegreeLevelId.HasValue)
        {
            queryable = queryable.Where(s => s.LevelId == request.DegreeLevelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            queryable = queryable.Where(s => s.Code.Contains(request.Code, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            queryable = queryable.Where(s => s.Title.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
        }

        var dtos = queryable.Select(s => new SpecialityDto
        {
            Id = s.Id,
            DegreeLevelId = s.LevelId,
            Code = s.Code,
            Name = s.Title,
            IsDeleted = s.Deleted
        }).ToList();

        return Result.Success<IReadOnlyList<SpecialityDto>>(dtos);
    }
}
