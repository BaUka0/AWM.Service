namespace AWM.Service.Application.Features.Org.Queries.Institutes.GetAllInstitutes;

using AWM.Service.Application.Features.Org.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all institutes for a specific university.
/// </summary>
public sealed class GetAllInstitutesQueryHandler
    : IRequestHandler<GetAllInstitutesQuery, Result<IReadOnlyList<InstituteDto>>>
{
    private readonly IUniversityRepository _universityRepository;

    public GetAllInstitutesQueryHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
    }

    public async Task<Result<IReadOnlyList<InstituteDto>>> Handle(
        GetAllInstitutesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var university = await _universityRepository.GetByIdAsync(request.UniversityId, cancellationToken);

            if (university is null)
            {
                return Result.Failure<IReadOnlyList<InstituteDto>>(
                    new Error("404", $"University with ID {request.UniversityId} not found."));
            }

            var instituteDtos = university.Institutes
                .Where(i => !i.IsDeleted)
                .Select(i => MapToDto(i, request.IncludeDepartments))
                .ToList();

            return Result.Success<IReadOnlyList<InstituteDto>>(instituteDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<InstituteDto>>(
                new Error("500", $"An error occurred while retrieving institutes: {ex.Message}"));
        }
    }

    private static InstituteDto MapToDto(Domain.Org.Entities.Institute institute, bool includeDepartments)
    {
        return new InstituteDto
        {
            Id = institute.Id,
            UniversityId = institute.UniversityId,
            Name = institute.Name,
            CreatedAt = institute.CreatedAt,
            CreatedBy = institute.CreatedBy,
            LastModifiedAt = institute.LastModifiedAt,
            LastModifiedBy = institute.LastModifiedBy,
            Departments = includeDepartments
                ? institute.Departments
                    .Where(d => !d.IsDeleted)
                    .Select(d => new DepartmentDto
                    {
                        Id = d.Id,
                        InstituteId = d.InstituteId,
                        Name = d.Name,
                        Code = d.Code,
                        CreatedAt = d.CreatedAt,
                        CreatedBy = d.CreatedBy,
                        LastModifiedAt = d.LastModifiedAt,
                        LastModifiedBy = d.LastModifiedBy
                    })
                    .ToList()
                : null
        };
    }
}