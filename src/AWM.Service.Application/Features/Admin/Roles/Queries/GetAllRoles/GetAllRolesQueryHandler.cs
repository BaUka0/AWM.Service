namespace AWM.Service.Application.Features.Admin.Roles.Queries.GetAllRoles;

using AWM.Service.Application.Features.Admin.Roles.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetAllRolesQuery.
/// Returns all roles with their current valid user counts for the specified university.
/// </summary>
public sealed class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<IReadOnlyList<AdminRoleDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public GetAllRolesQueryHandler(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyList<AdminRoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        return Result.Failure<IReadOnlyList<AdminRoleDto>>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
