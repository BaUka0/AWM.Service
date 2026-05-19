namespace AWM.Service.Application.Features.Org.Institutes.Queries.GetInstituteById;

using AWM.Service.Application.Features.Org.Departments.DTOs;

using AWM.Service.Application.Features.Org.Institutes.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a specific institute by ID.
/// </summary>
public sealed class GetInstituteByIdQueryHandler
    : IRequestHandler<GetInstituteByIdQuery, Result<InstituteDto>>
{
    private readonly IOrganizationLookupRepository _organizationLookupRepository;

    public GetInstituteByIdQueryHandler(IOrganizationLookupRepository organizationLookupRepository)
    {
        _organizationLookupRepository = organizationLookupRepository ?? throw new ArgumentNullException(nameof(organizationLookupRepository));
    }

    public async Task<Result<InstituteDto>> Handle(
        GetInstituteByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var institute = await _organizationLookupRepository.GetInstituteByIdAsync(request.InstituteId, cancellationToken);

            if (institute is null || institute.Deleted)
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

    private static InstituteDto MapToDto(Domain.University.OrgUnit institute, bool includeDepartments)
    {
        return new InstituteDto
        {
            Id = institute.Id,
            Name = institute.Title,
            CreatedAt = default,
            CreatedBy = 0,
            LastModifiedAt = null,
            LastModifiedBy = null,
            Departments = includeDepartments
                ? institute.Children
                    .Where(d => !d.Deleted)
                    .Select(d => new DepartmentDto
                    {
                        Id = d.Id,
                        InstituteId = d.ParentId ?? 0,
                        Name = d.Title,
                        Code = d.ShortTitle,
                        CreatedAt = default,
                        CreatedBy = 0,
                        LastModifiedAt = null,
                        LastModifiedBy = null
                    })
                    .ToList()
                : null
        };
    }
}