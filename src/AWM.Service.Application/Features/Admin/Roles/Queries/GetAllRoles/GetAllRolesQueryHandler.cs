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
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var users = await _userRepository.GetByUniversityAsync(request.UniversityId, cancellationToken);

        var result = new List<AdminRoleDto>();

        foreach (var role in roles)
        {
            // Count users who have this role currently assigned
            var usersCount = users.Count(u => u.RoleAssignments.Any(ra => ra.RoleId == role.Id && ra.IsCurrentlyValid()));

            result.Add(new AdminRoleDto
            {
                RoleId = role.Id,
                SystemName = role.SystemName,
                DisplayName = role.DisplayName ?? role.SystemName,
                ScopeLevel = role.ScopeLevel.ToString(),
                UsersCount = usersCount
            });
        }

        return Result.Success<IReadOnlyList<AdminRoleDto>>(result);
    }
}
