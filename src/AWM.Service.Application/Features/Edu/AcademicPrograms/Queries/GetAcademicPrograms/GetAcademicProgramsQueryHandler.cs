namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Queries.GetAcademicPrograms;

using AWM.Service.Application.Features.Edu.AcademicPrograms.DTOs;
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
public sealed class GetAcademicProgramsQueryHandler 
    : IRequestHandler<GetAcademicProgramsQuery, Result<IReadOnlyList<AcademicProgramDto>>>
{
    private readonly IAcademicProgramRepository _academicProgramRepository;

    public GetAcademicProgramsQueryHandler(
        IAcademicProgramRepository academicProgramRepository)
    {
        _academicProgramRepository = academicProgramRepository;
    }

    public async Task<Result<IReadOnlyList<AcademicProgramDto>>> Handle(
        GetAcademicProgramsQuery request, 
        CancellationToken cancellationToken)
    {
        var specialities = await _academicProgramRepository.GetAllAsync(cancellationToken);
        
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

        var dtos = queryable.Select(s => new AcademicProgramDto
        {
            Id = s.Id,
            DegreeLevelId = s.LevelId,
            Code = s.Code,
            Name = s.Title,
            IsDeleted = s.Deleted
        }).ToList();

        return Result.Success<IReadOnlyList<AcademicProgramDto>>(dtos);
    }
}