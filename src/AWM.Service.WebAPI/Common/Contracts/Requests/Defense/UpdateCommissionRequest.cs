namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for updating a commission's name.
/// </summary>
public sealed record UpdateCommissionRequest
{
    /// <summary>
    /// New name for the commission.
    /// </summary>
    /// <example>Государственная аттестационная комиссия 2024</example>
    public string Name { get; init; } = null!;
}
