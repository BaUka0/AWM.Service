namespace AWM.Service.Domain.Auth.RbacPlus.ViewModels;

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
