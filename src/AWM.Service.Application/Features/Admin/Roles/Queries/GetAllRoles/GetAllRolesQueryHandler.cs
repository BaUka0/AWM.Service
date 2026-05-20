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
    private readonly AWM.Service.Domain.Auth.Repositories.IUserAccessRepository _userAccessRepository;

    public GetAllRolesQueryHandler(
        IRoleRepository roleRepository,
        AWM.Service.Domain.Auth.Repositories.IUserAccessRepository userAccessRepository)
    {
        _roleRepository = roleRepository;
        _userAccessRepository = userAccessRepository;
    }

    public async Task<Result<IReadOnlyList<AdminRoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var userAccesses = await _userAccessRepository.GetAllAsync(cancellationToken);

        var activeUsersCount = userAccesses
            .GroupBy(ua => ua.RoleAccessId)
            .ToDictionary(g => g.Key, g => g.Select(ua => ua.UserId).Distinct().Count());

        var dtos = roles.Select(r => new AdminRoleDto
        {
            RoleId = r.Id,
            SystemName = r.Code,
            DisplayName = r.NameRu, // Using NameRu as DisplayName
            ScopeLevel = "Global", // Default scope level for RBAC+ roles
            UsersCount = activeUsersCount.TryGetValue(r.Id, out var count) ? count : 0
        }).ToList();

        return Result.Success<IReadOnlyList<AdminRoleDto>>(dtos);
    }
}
