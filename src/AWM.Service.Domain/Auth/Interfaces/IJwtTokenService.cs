using AWM.Service.Domain.Auth.Entities;

namespace AWM.Service.Domain.Auth.Interfaces;

/// <summary>
/// Interface for JWT token generation.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user with their roles.
    /// </summary>
    string GenerateToken(User user, IEnumerable<string> roles);

    /// <summary>
    /// Generates a secure random refresh token and its expiration time.
    /// </summary>
    (string Token, DateTime Expiry) GenerateRefreshToken();
}
