namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Auth.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetCurrentUserQuery.
/// Retrieves currently logged in user details, roles, and department ID.
/// </summary>
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserResult>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

    public GetCurrentUserQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IUserRepository userRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IEmployeeReadOnlyRepository employeeReadOnlyRepository)
    {
        _currentUserProvider = currentUserProvider;
        _userRepository = userRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _employeeReadOnlyRepository = employeeReadOnlyRepository;
    }

    public async Task<Result<UserResult>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        if (userId == null)
        {
            return Result.Failure<UserResult>(new Error(ErrorCodes.AuthUnauthorized, "Пользователь не авторизован."));
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user == null)
        {
            return Result.Failure<UserResult>(new Error(ErrorCodes.AuthUserNotFound, "Пользователь не найден."));
        }

        var userAccesses = await _userAccessRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var allRoles = await _roleAccessRepository.GetAllAsync(cancellationToken);
        var userRoleIds = userAccesses.Select(ua => ua.RoleAccessId).ToHashSet();
        var roles = allRoles.Where(r => userRoleIds.Contains(r.Id)).Select(r => r.Code).ToList();

        var employee = await _employeeReadOnlyRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var orgUnitId = employee?.Positions?.FirstOrDefault(p => p.IsMainPosition)?.OrgUnitId 
                           ?? employee?.Positions?.FirstOrDefault()?.OrgUnitId;

        var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();

        return Result.Success(new UserResult(
            user.Id,
            user.Email ?? string.Empty,
            user.Email ?? string.Empty,
            fullName,
            roles,
            orgUnitId
        ));
    }
}
