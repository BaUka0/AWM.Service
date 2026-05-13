namespace AWM.Service.WebAPI.Common.Contracts.Requests.Users;

/// <summary>
/// Request to create a new user by administrator.
/// </summary>
public sealed record CreateUserRequest(
    string Login,
    string Email,
    string Password,
    int RoleId,
    int? DepartmentId,
    int? InstituteId,
    int UniversityId);
