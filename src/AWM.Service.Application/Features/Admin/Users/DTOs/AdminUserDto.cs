namespace AWM.Service.Application.Features.Admin.Users.DTOs;

/// <summary>
/// Internal DTO for admin user list/detail operations.
/// </summary>
public sealed class AdminUserDto
{
    public int UserId { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public int? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public int? RoleId { get; init; }
    public DateTime CreatedAt { get; init; }
}
