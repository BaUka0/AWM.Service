namespace AWM.Service.WebAPI.Common.Contracts.Requests.Users;

/// <summary>
/// Request to update an existing user by administrator.
/// </summary>
public sealed record UpdateUserRequest(
    string Email,
    int RoleId,
    int? DepartmentId,
    int? InstituteId);
