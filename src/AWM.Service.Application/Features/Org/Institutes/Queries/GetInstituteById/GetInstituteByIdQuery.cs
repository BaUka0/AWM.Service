namespace AWM.Service.Application.Features.Org.Institutes.Queries.GetInstituteById;

using AWM.Service.Application.Features.Org.Institutes.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve a specific institute by its ID.
/// </summary>
public sealed record GetInstituteByIdQuery : IRequest<Result<InstituteDto>>
{
    public int InstituteId { get; init; }
    public bool IncludeDepartments { get; init; }
}