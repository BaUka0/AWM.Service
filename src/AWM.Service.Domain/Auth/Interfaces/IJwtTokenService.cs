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
}
