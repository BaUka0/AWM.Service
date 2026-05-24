namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleAccesses;

using AWM.Service.Domain.Auth.Entities;
using MediatR;

public sealed record GetAllRoleAccessesQuery : IRequest<IReadOnlyList<RoleAccess>>;
