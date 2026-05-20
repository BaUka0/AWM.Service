namespace AWM.Service.Application.Features.Admin.Users.Queries.GetAllUsers;

using AWM.Service.Application.Features.Admin.Users.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetAllUsersQuery.
/// Returns all users for a university with optional active/search filters.
/// </summary>
public sealed class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, Result<(IReadOnlyList<AdminUserDto> Items, int TotalCount)>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;
    private readonly AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository _localAccountRepository;
    private readonly AWM.Service.Domain.Auth.Repositories.IUserAccessRepository _userAccessRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IEmployeeRepository _EmployeeRepository;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        IOrganizationLookupRepository orgLookupRepository,
        AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository localAccountRepository,
        AWM.Service.Domain.Auth.Repositories.IUserAccessRepository userAccessRepository,
        IRoleRepository roleRepository,
        IEmployeeRepository EmployeeRepository)
    {
        _userRepository = userRepository;
        _orgLookupRepository = orgLookupRepository;
        _localAccountRepository = localAccountRepository;
        _userAccessRepository = userAccessRepository;
        _roleRepository = roleRepository;
        _EmployeeRepository = EmployeeRepository;
    }

    public async Task<Result<(IReadOnlyList<AdminUserDto> Items, int TotalCount)>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLowerInvariant();
            users = users.Where(u => 
                (u.LastName != null && u.LastName.ToLowerInvariant().Contains(searchLower)) ||
                (u.FirstName != null && u.FirstName.ToLowerInvariant().Contains(searchLower)) ||
                (u.MiddleName != null && u.MiddleName.ToLowerInvariant().Contains(searchLower)) ||
                (u.Email != null && u.Email.ToLowerInvariant().Contains(searchLower)) ||
                (u.IIN != null && u.IIN.Contains(searchLower))
            ).ToList();
        }

        var localAccounts = (await _localAccountRepository.GetAllAsync(cancellationToken)).ToDictionary(la => la.UserId);

        if (request.IsActive.HasValue)
        {
            users = users.Where(u => {
                localAccounts.TryGetValue(u.Id, out var la);
                var isActive = la?.IsActive ?? false;
                return isActive == request.IsActive.Value;
            }).ToList();
        }

        var userAccesses = await _userAccessRepository.GetAllAsync(cancellationToken);
        var roles = (await _roleRepository.GetAllAsync(cancellationToken)).ToDictionary(r => r.Id);
        var employees = (await _EmployeeRepository.GetAllWithPositionsAsync(cancellationToken)).ToDictionary(e => e.Id);

        var userAccessLookup = userAccesses.GroupBy(ua => ua.UserId).ToDictionary(g => g.Key, g => g.ToList());

        var dtos = users.Select(user =>
        {
            localAccounts.TryGetValue(user.Id, out var localAccount);
            userAccessLookup.TryGetValue(user.Id, out var accessList);
            accessList ??= new List<AWM.Service.Domain.Auth.Entities.UserAccess>();

            var userRoles = accessList.Select(ua => roles.TryGetValue(ua.RoleAccessId, out var r) ? r.Code : string.Empty).Where(code => !string.IsNullOrEmpty(code)).ToList();
            var primaryRole = accessList.FirstOrDefault()?.RoleAccessId;

            int? departmentId = null;
            string? departmentName = null;

            if (employees.TryGetValue(user.Id, out var employee))
            {
                var mainPos = employee.Positions.FirstOrDefault(p => p.IsMainPosition) ?? employee.Positions.FirstOrDefault();
                if (mainPos != null)
                {
                    departmentId = mainPos.OrgUnitId;
                    departmentName = mainPos.OrgUnit?.Title;
                }
            }

            return new AdminUserDto
            {
                UserId = user.Id,
                Login = user.Email ?? string.Empty,
                Email = user.Email ?? string.Empty,
                IsActive = localAccount?.IsActive ?? false,
                Roles = userRoles,
                RoleId = primaryRole,
                DepartmentId = departmentId,
                DepartmentName = departmentName,
                CreatedAt = localAccount?.CreatedAt ?? default
            };
        }).ToList();

        var totalCount = dtos.Count;
        var paginatedDtos = dtos
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return Result.Success<(IReadOnlyList<AdminUserDto> Items, int TotalCount)>((paginatedDtos, totalCount));
    }
}
