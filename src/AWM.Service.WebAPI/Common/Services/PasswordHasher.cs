using AWM.Service.Domain.Auth.Interfaces;

namespace AWM.Service.WebAPI.Common.Services;

/// <summary>
/// BCrypt implementation of password hashing.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <inheritdoc/>
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <inheritdoc/>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
