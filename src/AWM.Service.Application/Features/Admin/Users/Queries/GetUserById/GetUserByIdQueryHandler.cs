namespace AWM.Service.Application.Features.Admin.Users.Queries.GetUserById;

using AWM.Service.Application.Features.Admin.Users.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetUserByIdQuery.
/// Returns a single user with their role and department context.
/// </summary>
public sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, Result<AdminUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;
    private readonly AWM.Service.Domain.Auth.Repositories.ILocalAccountRepository _localAccountRepository;
    private readonly AWM.Service.Domain.Auth.Repositories.IUserAccessRepository _userAccessRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IEmployeeRepository _EmployeeRepository;

    public GetUserByIdQueryHandler(
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

    public async Task<Result<AdminUserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<AdminUserDto>(new Error("Admin.UserNotFound", "Пользователь не найден."));
        }

        var localAccount = await _localAccountRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var userAccesses = await _userAccessRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var roles = (await _roleRepository.GetAllAsync(cancellationToken)).ToDictionary(r => r.Id);
        var employee = await _EmployeeRepository.GetByIdWithPositionsAsync(user.Id, cancellationToken);

        var userRoles = userAccesses.Select(ua => roles.TryGetValue(ua.RoleAccessId, out var r) ? r.Code : string.Empty).Where(code => !string.IsNullOrEmpty(code)).ToList();
        var primaryRole = userAccesses.FirstOrDefault()?.RoleAccessId;

        int? departmentId = null;
        string? departmentName = null;

        if (employee != null)
        {
            var mainPos = employee.Positions.FirstOrDefault(p => p.IsMainPosition) ?? employee.Positions.FirstOrDefault();
            if (mainPos != null)
            {
                departmentId = mainPos.OrgUnitId;
                departmentName = mainPos.OrgUnit?.Title;
            }
        }

        var dto = new AdminUserDto
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

        return Result.Success(dto);
    }
}
