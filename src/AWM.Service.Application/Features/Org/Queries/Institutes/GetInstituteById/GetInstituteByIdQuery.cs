namespace AWM.Service.Application.Features.Org.Queries.Institutes.GetInstituteById;

using AWM.Service.Application.Features.Org.DTOs;
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