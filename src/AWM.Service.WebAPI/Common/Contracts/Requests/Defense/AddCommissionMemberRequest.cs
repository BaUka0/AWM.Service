namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for adding a member to a commission.
/// </summary>
public sealed record AddCommissionMemberRequest
{
    /// <summary>
    /// User ID of the member to add.
    /// </summary>
    /// <example>42</example>
    public int UserId { get; init; }

    /// <summary>
    /// Role of the member in the commission (1 = Chairman, 2 = Secretary, 3 = Member).
    /// </summary>
    /// <example>1</example>
    public int RoleInCommission { get; init; }
}
