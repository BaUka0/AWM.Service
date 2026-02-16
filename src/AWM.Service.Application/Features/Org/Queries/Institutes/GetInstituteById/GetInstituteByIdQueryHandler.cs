namespace AWM.Service.Application.Features.Org.Queries.Institutes.GetInstituteById;

using AWM.Service.Application.Features.Org.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a specific institute by ID.
/// </summary>
public sealed class GetInstituteByIdQueryHandler
    : IRequestHandler<GetInstituteByIdQuery, Result<InstituteDto>>
{
    private readonly IUniversityRepository _universityRepository;

    public GetInstituteByIdQueryHandler(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository ?? throw new ArgumentNullException(nameof(universityRepository));
    }

    public async Task<Result<InstituteDto>> Handle(
        GetInstituteByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var universities = await _universityRepository.GetAllAsync(cancellationToken);

            var university = universities.FirstOrDefault(u =>
                u.Institutes.Any(i => i.Id == request.InstituteId && !i.IsDeleted));

            if (university is null)
            {
                return Result.Failure<InstituteDto>(
                    new Error("404", $"Institute with ID {request.InstituteId} not found."));
            }

            var institute = university.Institutes.FirstOrDefault(i => i.Id == request.InstituteId);

            if (institute is null || institute.IsDeleted)
            {
                return Result.Failure<InstituteDto>(
                    new Error("404", $"Institute with ID {request.InstituteId} not found or has been deleted."));
            }

            var instituteDto = MapToDto(institute, request.IncludeDepartments);

            return Result.Success(instituteDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<InstituteDto>(
                new Error("500", $"An error occurred while retrieving the institute: {ex.Message}"));
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