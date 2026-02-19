namespace AWM.Service.Application.Features.Org.Queries.Departments.GetDepartmentsByInstitute;

using AWM.Service.Application.Features.Org.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all departments belonging to a specific institute.
/// </summary>
public sealed class GetDepartmentsByInstituteQueryHandler
    : IRequestHandler<GetDepartmentsByInstituteQuery, Result<IReadOnlyList<DepartmentDto>>>
{
    private readonly IUniversityRepository _universityRepository;

    public GetDepartmentsByInstituteQueryHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>>> Handle(
        GetDepartmentsByInstituteQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var university = await _universityRepository.GetByInstituteIdAsync(request.InstituteId, cancellationToken);

            if (university is null)
            {
                return Result.Failure<IReadOnlyList<DepartmentDto>>(
                    new Error("NotFound.Institute", $"Institute with ID {request.InstituteId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i => i.Id == request.InstituteId);

            if (institute is null || institute.IsDeleted)
            {
                return Result.Failure<IReadOnlyList<DepartmentDto>>(
                    new Error("NotFound.Institute", $"Institute with ID {request.InstituteId} not found or has been deleted."));
            }

            var departmentDtos = institute.Departments
                .Where(d => !d.IsDeleted)
                .Select(MapToDto)
                .ToList();

            return Result.Success<IReadOnlyList<DepartmentDto>>(departmentDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<DepartmentDto>>(
                new Error("InternalError", $"An error occurred while retrieving departments: {ex.Message}"));
        }
    }

    private static DepartmentDto MapToDto(Domain.Org.Entities.Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            InstituteId = department.InstituteId,
            Name = department.Name,
            Code = department.Code,
            CreatedAt = department.CreatedAt,
            CreatedBy = department.CreatedBy,
            LastModifiedAt = department.LastModifiedAt,
            LastModifiedBy = department.LastModifiedBy
        };
    }
}