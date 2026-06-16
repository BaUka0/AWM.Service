namespace AWM.Service.WebAPI.Common.Contracts.Responses;

using System.Collections.Generic;

/// <summary>
/// Response contract for current user details.
/// </summary>
public record UserResponse
{
    public int UserId { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public IEnumerable<string> Roles { get; init; } = Array.Empty<string>();
    public int? OrgUnitId { get; init; }
    public int? CurrentSemesterId { get; init; }
}
