namespace AWM.Service.Application.Features.Org.Institutes.Queries.GetAllInstitutes;

using AWM.Service.Application.Features.Org.Institutes.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all institutes (non-deleted) for a specific university.
/// </summary>
public sealed record GetAllInstitutesQuery : IRequest<Result<IReadOnlyList<InstituteDto>>>
{
    public int UniversityId { get; init; }
    public bool IncludeDepartments { get; init; }
}