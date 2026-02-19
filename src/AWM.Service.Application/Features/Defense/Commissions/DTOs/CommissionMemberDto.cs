namespace AWM.Service.Application.Features.Defense.Commissions.DTOs;

/// <summary>
/// Data Transfer Object for CommissionMember entity.
/// </summary>
public sealed record CommissionMemberDto
{
    /// <summary>
    /// Member record ID.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Commission ID this member belongs to.
    /// </summary>
    public int CommissionId { get; init; }

    /// <summary>
    /// User ID of the member.
    /// </summary>
    public int UserId { get; init; }

    /// <summary>
    /// Role in commission (Chairman, Secretary, Member).
    /// </summary>
    public string RoleInCommission { get; init; } = null!;

    /// <summary>
    /// Date and time when the member was added.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
