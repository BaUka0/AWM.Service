namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleActionTypes;

using AWM.Service.Domain.Auth.Entities;
using MediatR;

public sealed record GetAllRoleActionTypesQuery : IRequest<IReadOnlyList<RoleActionType>>;
