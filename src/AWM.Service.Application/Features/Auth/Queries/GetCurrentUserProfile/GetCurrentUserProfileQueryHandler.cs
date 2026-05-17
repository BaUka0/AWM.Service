namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using AWM.Service.Application.Features.Auth.DTOs;

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
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;

    public GetCurrentUserProfileQueryHandler(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IStaffRepository staffRepository,
        IReviewerRepository reviewerRepository,
        IAcademicYearRepository academicYearRepository,
        IOrganizationLookupRepository orgLookupRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _staffRepository = staffRepository;
        _reviewerRepository = reviewerRepository;
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
        var roles = user.UserAccesses
            .Select(ua => ua.RoleAccess?.Code ?? ua.RoleAccessId.ToString())
            .Distinct()
            .ToList();

        // 3. Department/institute context no longer available from role assignments in RBAC+
        int? departmentId = null;
        int? instituteId = null;

        string? departmentName = null;
        string? instituteName = null;

        // 5. Resolve current academic year
        var currentYear = await _academicYearRepository.GetCurrentAsync(cancellationToken);

        // 6. Load staff profile if user has a staff-related role
        int? staffId = null;
        string? position = null;
        string? academicDegree = null;
        bool? isSupervisor = null;

        var staffRoles = new[]
        {
            "Supervisor",
            "HeadOfDepartment",
            "Secretary",
            "Expert",
            "CommissionMember",
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

                departmentId ??= staff.DepartmentId;
            }
        }

        if (departmentId.HasValue)
        {
            departmentName = (await _orgLookupRepository.GetDepartmentByIdAsync(departmentId.Value, cancellationToken))?.Name;
        }

        if (instituteId.HasValue)
        {
            instituteName = (await _orgLookupRepository.GetInstituteByIdAsync(instituteId.Value, cancellationToken))?.Name;
        }

        // 7. Load reviewer profile if user has Reviewer role
        int? reviewerId = null;

        var isReviewer = roles.Contains("Reviewer");
        if (isReviewer)
        {
            var reviewer = await _reviewerRepository.GetByUserIdAsync(user.Id, cancellationToken);
            reviewerId = reviewer?.Id;
        }

        // 8. Load student profile if user has Student role
        int? studentId = null;
        string? groupCode = null;

        var isStudent = roles.Contains("Student");
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
            ReviewerId = reviewerId,
            StudentId = studentId,
            GroupCode = groupCode,
        };

        return Result.Success(profile);
    }
}
