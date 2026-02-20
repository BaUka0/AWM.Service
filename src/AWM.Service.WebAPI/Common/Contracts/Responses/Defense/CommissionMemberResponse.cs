namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for commission member data.
/// </summary>
public sealed record CommissionMemberResponse
{
    /// <summary>
    /// Member record ID.
    /// </summary>
    /// <example>10</example>
    public int Id { get; init; }

    /// <summary>
    /// Commission ID this member belongs to.
    /// </summary>
    /// <example>1</example>
    public int CommissionId { get; init; }

    /// <summary>
    /// User ID of the member.
    /// </summary>
    /// <example>42</example>
    public int UserId { get; init; }

    /// <summary>
    /// Role in commission (Chairman, Secretary, Member).
    /// </summary>
    /// <example>Chairman</example>
    public string RoleInCommission { get; init; } = null!;

    /// <summary>
    /// Date and time when the member was added.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; init; }
}
