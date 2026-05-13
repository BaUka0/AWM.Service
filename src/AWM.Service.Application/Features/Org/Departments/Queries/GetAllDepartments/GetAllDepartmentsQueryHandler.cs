namespace AWM.Service.Application.Features.Org.Departments.Queries.GetAllDepartments;

using AWM.Service.Application.Features.Org.Departments.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all departments belonging to a specific university.
/// </summary>
public sealed class GetAllDepartmentsQueryHandler
    : IRequestHandler<GetAllDepartmentsQuery, Result<IReadOnlyList<DepartmentDto>>>
{
    private readonly IUniversityRepository _universityRepository;

    public GetAllDepartmentsQueryHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
    }

    public async Task<Result<IReadOnlyList<DepartmentDto>>> Handle(
        GetAllDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var university = await _universityRepository.GetByIdAsync(request.UniversityId, cancellationToken);

            if (university is null)
            {
                return Result.Failure<IReadOnlyList<DepartmentDto>>(
                    new Error("NotFound.University", $"University with ID {request.UniversityId} not found."));
            }

            var departmentDtos = university.Institutes
                .Where(i => !i.IsDeleted)
                .SelectMany(i => i.Departments)
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
