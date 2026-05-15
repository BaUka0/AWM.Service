namespace AWM.Service.Application.Features.Admin.Roles.DTOs;

/// <summary>
/// Internal DTO for admin role list operations.
/// </summary>
public sealed class AdminRoleDto
{
    public int RoleId { get; init; }
    public string SystemName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string ScopeLevel { get; init; } = string.Empty;
    public int UsersCount { get; init; }
}
