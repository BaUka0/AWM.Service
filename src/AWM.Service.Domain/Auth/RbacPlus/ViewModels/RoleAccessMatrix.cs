namespace AWM.Service.Domain.Auth.RbacPlus.ViewModels;

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
