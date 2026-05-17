namespace AWM.Service.Domain.Auth.RbacPlus.ViewModels;

/// <summary>
/// Database view: lightweight version for quick role check.
/// Mapped as HasNoKey entity.
/// </summary>
public class ReducedUserAccessMatrix
{
    public int UserId { get; set; }
    public string RoleCode { get; set; } = null!;
}
