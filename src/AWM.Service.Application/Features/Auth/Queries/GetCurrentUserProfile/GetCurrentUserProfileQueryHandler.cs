namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using AWM.Service.Application.Features.Auth.DTOs;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetCurrentUserProfileQuery.
/// Returns full profile data for the authenticated user.
/// </summary>
public sealed class GetCurrentUserProfileQueryHandler
    : IRequestHandler<GetCurrentUserProfileQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;

    public GetCurrentUserProfileQueryHandler(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IStaffRepository staffRepository,
        IAcademicYearRepository academicYearRepository,
        IOrganizationLookupRepository orgLookupRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _staffRepository = staffRepository;
        _academicYearRepository = academicYearRepository;
        _orgLookupRepository = orgLookupRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetCurrentUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Load user with role assignments
        var user = await _userRepository.GetWithRoleAssignmentsAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserProfileDto>(new Error("NotFound.User", "Пользователь не найден."));
        }

        // 2. Resolve roles
        var roles = user.RoleAssignments
            .Where(ra => ra.IsCurrentlyValid())
            .Select(ra => ra.Role?.SystemName ?? ra.RoleId.ToString())
            .Distinct()
            .ToList();

        // 3. Resolve department context from the first valid scoped role assignment
        var scopedAssignment = user.RoleAssignments
            .FirstOrDefault(ra => ra.IsCurrentlyValid() && ra.DepartmentId.HasValue);

        int? departmentId = scopedAssignment?.DepartmentId;
        int? instituteId = scopedAssignment?.InstituteId;

        // 4. Load department and institute names if available
        string? departmentName = null;
        string? instituteName = null;

        if (departmentId.HasValue)
        {
            var department = await _orgLookupRepository.GetDepartmentByIdAsync(departmentId.Value, cancellationToken);
            departmentName = department?.Name;
        }

        if (instituteId.HasValue)
        {
            var institute = await _orgLookupRepository.GetInstituteByIdAsync(instituteId.Value, cancellationToken);
            instituteName = institute?.Name;
        }

        // 5. Resolve current academic year
        var currentYear = await _academicYearRepository.GetCurrentAsync(user.UniversityId, cancellationToken);

        // 6. Load staff profile if user has a staff-related role
        int? staffId = null;
        string? position = null;
        string? academicDegree = null;
        bool? isSupervisor = null;

        var staffRoles = new[]
        {
            nameof(RoleType.Supervisor),
            nameof(RoleType.HeadOfDepartment),
            nameof(RoleType.Secretary),
            nameof(RoleType.Expert),
            nameof(RoleType.CommissionMember),
        };
        var isStaff = roles.Any(r => staffRoles.Contains(r));
        if (isStaff)
        {
            var staff = await _staffRepository.GetByUserIdAsync(user.Id, cancellationToken);
            if (staff is not null)
            {
                staffId = staff.Id;
                position = staff.Position;
                academicDegree = staff.AcademicDegree;
                isSupervisor = staff.IsSupervisor;

                // Use staff's department as fallback if not resolved from role assignment
                departmentId ??= staff.DepartmentId;
                if (departmentId.HasValue && departmentName is null)
                {
                    var dept = await _orgLookupRepository.GetDepartmentByIdAsync(departmentId.Value, cancellationToken);
                    departmentName = dept?.Name;
                }
            }
        }

        // 7. Load student profile if user has Student role
        int? studentId = null;
        string? groupCode = null;

        var isStudent = roles.Contains(nameof(RoleType.Student));
        if (isStudent)
        {
            var student = await _studentRepository.GetByUserIdAsync(user.Id, cancellationToken);
            studentId = student?.Id;
            groupCode = student?.GroupCode;
        }

        var profile = new UserProfileDto
        {
            UserId = user.Id,
            Login = user.Login,
            Email = user.Email,
            Roles = roles,
            DepartmentId = departmentId,
            DepartmentName = departmentName,
            InstituteId = instituteId,
            InstituteName = instituteName,
            CurrentAcademicYearId = currentYear?.Id,
            CurrentAcademicYearName = currentYear?.Name,
            StaffId = staffId,
            Position = position,
            AcademicDegree = academicDegree,
            IsSupervisor = isSupervisor,
            StudentId = studentId,
            GroupCode = groupCode,
        };

        return Result.Success(profile);
    }
}
