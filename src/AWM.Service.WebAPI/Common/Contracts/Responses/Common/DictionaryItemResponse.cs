namespace AWM.Service.WebAPI.Common.Contracts.Responses.Common;

/// <summary>
/// Simple dictionary item response (id + name).
/// </summary>
public sealed record DictionaryItemResponse
{
    /// <summary>
    /// Item ID.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Display name.
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Optional code or additional identifier.
    /// </summary>
    public string? Code { get; init; }
}
