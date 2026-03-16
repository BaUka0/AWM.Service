namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for updating a research direction.
/// </summary>
public sealed record UpdateDirectionRequest
{
    /// <summary>
    /// Title in Russian (required).
    /// </summary>
    public string TitleRu { get; init; } = string.Empty;

    /// <summary>
    /// Title in Kazakh (optional).
    /// </summary>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Title in English (optional).
    /// </summary>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Direction description in Russian (optional).
    /// </summary>
    public string? DescriptionRu { get; init; }

    /// <summary>
    /// Direction description in Kazakh (optional).
    /// </summary>
    public string? DescriptionKz { get; init; }

    /// <summary>
    /// Direction description in English (optional).
    /// </summary>
    public string? DescriptionEn { get; init; }
}
