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
    private readonly IStudentReadOnlyRepository _studentReadOnlyRepository;
    private readonly ISpecialitySpecializationReadOnlyRepository _specSpecRepository;
    private readonly ISpecializationsOrgUnitReadOnlyRepository _specOrgRepository;
    private readonly ISemesterReadOnlyRepository _semesterReadOnlyRepository;

    public GetCurrentUserQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IUserRepository userRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IEmployeeReadOnlyRepository employeeReadOnlyRepository,
        IStudentReadOnlyRepository studentReadOnlyRepository,
        ISpecialitySpecializationReadOnlyRepository specSpecRepository,
        ISpecializationsOrgUnitReadOnlyRepository specOrgRepository,
        ISemesterReadOnlyRepository semesterReadOnlyRepository)
    {
        _currentUserProvider = currentUserProvider;
        _userRepository = userRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _employeeReadOnlyRepository = employeeReadOnlyRepository;
        _studentReadOnlyRepository = studentReadOnlyRepository;
        _specSpecRepository = specSpecRepository;
        _specOrgRepository = specOrgRepository;
        _semesterReadOnlyRepository = semesterReadOnlyRepository;
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

        // 1. Try to find as Employee
        var employee = await _employeeReadOnlyRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var orgUnitId = employee?.Positions?.FirstOrDefault(p => p.IsMainPosition)?.OrgUnitId
                           ?? employee?.Positions?.FirstOrDefault()?.OrgUnitId;

        // 2. If not found, try to find as Student
        if (orgUnitId == null)
        {
            var student = await _studentReadOnlyRepository.GetByUserIdAsync(user.Id, cancellationToken);
            if (student != null && student.SpecialityId.HasValue)
            {
                // Chain: Speciality -> Specialization -> OrgUnit
                var specSpecs = await _specSpecRepository.GetBySpecialityAsync(student.SpecialityId.Value, cancellationToken);
                var specId = specSpecs.FirstOrDefault()?.SpecializationId;
                
                if (specId.HasValue)
                {
                    var specOrgs = await _specOrgRepository.GetBySpecializationAsync(specId.Value, cancellationToken);
                    orgUnitId = specOrgs.FirstOrDefault()?.OrgUnitId;
                }
            }
        }

        var currentSemester = await _semesterReadOnlyRepository.GetCurrentAsync(cancellationToken);
        var currentSemesterId = currentSemester?.Id;
        if (currentSemesterId == null)
        {
            var semesters = await _semesterReadOnlyRepository.GetAllAsync(cancellationToken);
            currentSemesterId = semesters.FirstOrDefault()?.Id;
        }

        var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();

        return Result.Success(new UserResult(
            user.Id,
            user.Email ?? string.Empty,
            user.Email ?? string.Empty,
            fullName,
            roles,
            orgUnitId,
            currentSemesterId
        ));
    }
}
