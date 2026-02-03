namespace AWM.Service.Domain.Auth.Interfaces;

/// <summary>
/// Interface for password hashing operations.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plaintext password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plaintext password against a hashed password.
    /// </summary>
    bool VerifyPassword(string password, string hashedPassword);
}
