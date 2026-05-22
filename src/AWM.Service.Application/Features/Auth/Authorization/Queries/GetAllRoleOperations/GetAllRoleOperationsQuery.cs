namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleOperations;

using AWM.Service.Domain.Auth.Entities;
using MediatR;

/// <summary>
/// Query to get all role operations (tree structure).
/// </summary>
public sealed record GetAllRoleOperationsQuery : IRequest<IReadOnlyList<RoleOperation>>;
