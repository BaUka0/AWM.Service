namespace AWM.Service.Application.Features.Admin.Roles.Queries.GetAllRoles;

using AWM.Service.Application.Features.Admin.Roles.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all system roles with per-university user counts.
/// </summary>
public sealed record GetAllRolesQuery : IRequest<Result<IReadOnlyList<AdminRoleDto>>>
{
    public int UniversityId { get; init; }
}
