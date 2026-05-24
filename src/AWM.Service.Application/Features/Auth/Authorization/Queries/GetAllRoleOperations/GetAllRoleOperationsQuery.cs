namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleOperations;

using AWM.Service.Domain.Auth.Entities;
using MediatR;

public sealed record GetAllRoleOperationsQuery : IRequest<IReadOnlyList<RoleOperation>>;
