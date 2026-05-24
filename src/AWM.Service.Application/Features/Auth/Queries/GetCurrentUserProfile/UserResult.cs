namespace AWM.Service.Application.Features.Auth.Queries.GetCurrentUserProfile;

using System.Collections.Generic;

/// <summary>
/// Result DTO for retrieving currently authenticated user profile.
/// </summary>
public record UserResult(
    int UserId,
    string Login,
    string Email,
    string Name,
    IEnumerable<string> Roles,
    int? OrgUnitId,
    int? CurrentSemesterId
);
