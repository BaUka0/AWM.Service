namespace AWM.Service.Application.Features.Auth.DTOs;

/// <summary>
/// Result model for authentication operations.
/// </summary>
public record AuthResult(
    string Token,
    string Login,
    int UserId,
    string Email,
    IEnumerable<string> Roles,
    string RefreshToken
);
