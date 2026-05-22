namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleAccesses;

using AWM.Service.Domain.Auth.Entities;
using MediatR;

/// <summary>
/// Query to get all role access definitions.
/// </summary>
public sealed record GetAllRoleAccessesQuery : IRequest<IReadOnlyList<RoleAccess>>;
