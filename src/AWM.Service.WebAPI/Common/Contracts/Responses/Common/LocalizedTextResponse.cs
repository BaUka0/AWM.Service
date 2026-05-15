namespace AWM.Service.WebAPI.Common.Contracts.Responses.Common;

/// <summary>
/// A standardized structure for multilingual text.
/// Uses 'kk' instead of 'kz' to match standard frontend localization formats.
/// </summary>
public sealed record LocalizedTextResponse
{
    public string Ru { get; init; } = string.Empty;
    public string? Kk { get; init; }
    public string? En { get; init; }
}
