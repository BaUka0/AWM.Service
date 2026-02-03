namespace AWM.Service.WebAPI.Common.Contracts.Responses;

/// <summary>
/// Response model for successful login.
/// </summary>
public record LoginResponse(
    string Token,
    string Login,
    int UserId,
    string Email,
    IEnumerable<string> Roles
);
