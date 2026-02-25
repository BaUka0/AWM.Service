namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for assigning a student work to a defense schedule slot.
/// </summary>
public sealed record AssignWorkToSlotRequest
{
    /// <summary>
    /// StudentWork ID to assign to the slot.
    /// </summary>
    /// <example>42</example>
    public long WorkId { get; init; }
}
