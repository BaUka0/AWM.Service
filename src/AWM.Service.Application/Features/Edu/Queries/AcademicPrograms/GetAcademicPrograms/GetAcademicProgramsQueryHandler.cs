namespace AWM.Service.Application.Features.Edu.Queries.AcademicPrograms.GetAcademicPrograms;

using AWM.Service.Application.Features.Edu.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

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
        try
        {
            IReadOnlyList<Domain.Edu.Entities.AcademicProgram> programs;

            // Apply filters based on query parameters
            if (request.DepartmentId.HasValue)
            {
                programs = await _academicProgramRepository
                    .GetByDepartmentAsync(request.DepartmentId.Value, cancellationToken);
            }
            else if (request.DegreeLevelId.HasValue)
            {
                programs = await _academicProgramRepository
                    .GetByDegreeLevelAsync(request.DegreeLevelId.Value, cancellationToken);
            }
            else
            {
                // If no specific filter, we need a GetAllAsync method
                // For now, return empty list or implement GetAllAsync in repository
                return Result.Failure<IReadOnlyList<AcademicProgramDto>>(
                    new Error("Query.NotSupported", 
                        "Please provide at least DepartmentId or DegreeLevelId filter."));
            }

            // Apply in-memory filters
            var filtered = programs.AsEnumerable();

            // Filter by IsDeleted
            if (!request.IncludeDeleted)
            {
                filtered = filtered.Where(p => !p.IsDeleted);
            }

            // Filter by Code (partial match, case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                filtered = filtered.Where(p => 
                    p.Code != null && 
                    p.Code.Contains(request.Code, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by Name (partial match, case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                filtered = filtered.Where(p => 
                    p.Name != null && 
                    p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            }

            // Map to DTOs
            var result = filtered
                .Select(p => new AcademicProgramDto
                {
                    Id = p.Id,
                    DepartmentId = p.DepartmentId,
                    DegreeLevelId = p.DegreeLevelId,
                    Code = p.Code,
                    Name = p.Name,
                    CreatedAt = p.CreatedAt,
                    CreatedBy = p.CreatedBy,
                    LastModifiedAt = p.LastModifiedAt,
                    LastModifiedBy = p.LastModifiedBy,
                    IsDeleted = p.IsDeleted,
                    DeletedAt = p.DeletedAt,
                    DeletedBy = p.DeletedBy
                })
                .ToList();

            return Result.Success<IReadOnlyList<AcademicProgramDto>>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<AcademicProgramDto>>(
                new Error("InternalError", ex.Message));
        }
    }
}