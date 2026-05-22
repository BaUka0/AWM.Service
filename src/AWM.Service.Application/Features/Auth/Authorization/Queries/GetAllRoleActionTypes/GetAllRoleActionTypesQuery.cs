namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleActionTypes;

using AWM.Service.Domain.Auth.Entities;
using MediatR;

/// <summary>
/// Query to get all role action types.
/// </summary>
public sealed record GetAllRoleActionTypesQuery : IRequest<IReadOnlyList<RoleActionType>>;
