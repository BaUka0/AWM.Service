namespace AWM.Service.Application.Features.Org.Institutes.Queries.GetAllInstitutes;

using AWM.Service.Application.Features.Org.Departments.DTOs;

using AWM.Service.Application.Features.Org.Institutes.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all institutes for a specific university.
/// </summary>
public sealed class GetAllInstitutesQueryHandler
    : IRequestHandler<GetAllInstitutesQuery, Result<IReadOnlyList<InstituteDto>>>
{
    private readonly IOrganizationLookupRepository _organizationLookupRepository;

    public GetAllInstitutesQueryHandler(IOrganizationLookupRepository organizationLookupRepository)
    {
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
    }

    public async Task<Result<IReadOnlyList<InstituteDto>>> Handle(
        GetAllInstitutesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var institutes = await _organizationLookupRepository.GetAllInstitutesAsync(cancellationToken);

            var instituteDtos = institutes
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