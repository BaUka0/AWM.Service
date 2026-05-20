namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.University;

/// <summary>
/// Stores credentials and tokens for a local account mapped to a University User.
/// </summary>
public class LocalAccount : Entity<int>, IAuditable
{
    public int UserId { get; private set; } // FK -> Edu_Users.ID
    public string PasswordHash { get; private set; } = null!;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    // Navigation property
    public User User { get; private set; } = null!;

    private LocalAccount() { }

    public LocalAccount(int userId, string passwordHash, int createdBy)
    {
        if (userId <= 0)
            throw new DomainException("LocalAccount.InvalidUserId", "UserId must be positive.");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("LocalAccount.PasswordHashRequired", "Password hash is required.");

        UserId = userId;
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void UpdatePassword(string passwordHash, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("LocalAccount.PasswordHashRequired", "Password hash is required.");

        PasswordHash = passwordHash;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void SetRefreshToken(string? token, DateTime? expiryTime)
    {
        RefreshToken = token;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void ToggleStatus(bool isActive, int modifiedBy)
    {
        IsActive = isActive;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}
