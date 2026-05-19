namespace AWM.Service.Domain.Auth.ViewModels;

/// <summary>
/// Database view: lightweight version for quick role check.
/// Mapped as HasNoKey entity.
/// </summary>
public class ReducedUserAccessMatrix
{
    public int UserId { get; set; }
    public string RoleCode { get; set; } = null!;
}

/// <summary>
/// Database view: full role permission matrix.
/// Mapped as HasNoKey entity.
/// </summary>
public class RoleAccessMatrix
{
    public string RoleCode { get; set; } = null!;
    public string OperationName { get; set; } = null!;
    public string ActionTypeName { get; set; } = null!;
}

/// <summary>
/// Database view: full user permission matrix.
/// Mapped as HasNoKey entity.
/// </summary>
public class UserAccessMatrix
{
    public int UserId { get; set; }
    public string RoleCode { get; set; } = null!;
    public string OperationName { get; set; } = null!;
    public string ActionTypeName { get; set; } = null!;
}
