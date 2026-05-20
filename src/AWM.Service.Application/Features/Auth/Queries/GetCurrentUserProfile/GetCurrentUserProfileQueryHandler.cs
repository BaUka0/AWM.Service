namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using AWM.Service.Application.Features.Auth.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Auth.Repositories;
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
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly ISemesterRepository _semesterRepository;

    public GetCurrentUserProfileQueryHandler(
        IUserRepository userRepository,
        IStudentRepository studentRepository,
        IEmployeeRepository EmployeeRepository,
        IReviewerRepository reviewerRepository,
        IOrganizationLookupRepository orgLookupRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        ISemesterRepository semesterRepository)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _EmployeeRepository = EmployeeRepository;
        _reviewerRepository = reviewerRepository;
        _orgLookupRepository = orgLookupRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _semesterRepository = semesterRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetCurrentUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure<UserProfileDto>(new Error("Auth.UserNotFound", "Пользователь не найден."));
        }

        var userAccesses = await _userAccessRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var allRoles = await _roleAccessRepository.GetAllAsync(cancellationToken);
        var userRoleIds = userAccesses.Select(ua => ua.RoleAccessId).ToHashSet();
        var roles = allRoles.Where(r => userRoleIds.Contains(r.Id)).Select(r => r.Code).ToList();

        var currentSemester = await _semesterRepository.GetCurrentAsync(cancellationToken);

        var fullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
        var profile = new UserProfileDto
        {
            UserId = user.Id,
            Login = user.Email ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Name = fullName,
            Roles = roles,
            CurrentAcademicYearId = currentSemester?.Id,
            CurrentAcademicYearName = currentSemester?.Title
        };

        var student = await _studentRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (student != null)
        {
            profile = profile with
            {
                StudentId = student.Id,
                GroupCode = student.Speciality?.Code
            };
        }

        var employee = await _EmployeeRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (employee != null)
        {
            profile = profile with
            {
                StaffId = employee.Id,
                IsSupervisor = employee.IsAdvisor
            };
        }

        var reviewer = await _reviewerRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (reviewer != null)
        {
            profile = profile with
            {
                ReviewerId = reviewer.Id
            };
        }

        return Result.Success(profile);
    }
}

