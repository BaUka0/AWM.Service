namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// User entity for authentication and authorization.
/// Supports both internal and SSO (AD/Azure) authentication.
/// </summary>
public class User : AggregateRoot<int>, IAuditable, ISoftDeletable
{
    public int UniversityId { get; private set; }
    public string Login { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? PasswordHash { get; private set; }
    public string? ExternalId { get; private set; }
    public bool IsActive { get; private set; }

    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<UserRoleAssignment> _roleAssignments = new();
    public IReadOnlyCollection<UserRoleAssignment> RoleAssignments => _roleAssignments.AsReadOnly();

    private User() { }

    public User(int universityId, string login, string email, string? passwordHash = null, string? externalId = null)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Login is required.", nameof(login));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        UniversityId = universityId;
        Login = login;
        Email = email;
        PasswordHash = passwordHash;
        ExternalId = externalId;
        IsActive = true;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = 0; // System default
        LastModifiedAt = CreatedAt;
        LastModifiedBy = 0;
        IsDeleted = false;
    }

    /// <summary>
    /// Creates a user from SSO.
    /// </summary>
    public static User CreateFromSso(int universityId, string login, string email, string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("External ID is required for SSO users.", nameof(externalId));

        return new User(universityId, login, email, null, externalId);
    }

    /// <summary>
    /// Updates user email.
    /// </summary>
    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        Email = email;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates user password hash.
    /// </summary>
    public void UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Links user to SSO account.
    /// </summary>
    public void LinkToSso(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            throw new ArgumentException("External ID is required.", nameof(externalId));

        ExternalId = externalId;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the refresh token.
    /// </summary>
    public void UpdateRefreshToken(string token, DateTime expiryTime)
    {
        RefreshToken = token;
        RefreshTokenExpiryTime = expiryTime;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Revokes the refresh token.
    /// </summary>
    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns a role to the user.
    /// </summary>
    public UserRoleAssignment AssignRole(
        int roleId,
        int? departmentId = null,
        int? instituteId = null,
        int? academicYearId = null,
        int? assignedBy = null)
    {
        var assignment = new UserRoleAssignment(
            this.Id,
            roleId,
            departmentId,
            instituteId,
            academicYearId,
            assignedBy);

        _roleAssignments.Add(assignment);
        return assignment;
    }

    /// <summary>
    /// Soft deletes the user.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }

    public void SetAuditInfo(int createdBy)
    {
        CreatedBy = createdBy;
        LastModifiedBy = createdBy;
    }
}
