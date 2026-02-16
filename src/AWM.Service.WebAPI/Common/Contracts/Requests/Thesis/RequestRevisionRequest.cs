namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for requesting revision of a direction.
/// </summary>
public sealed record RequestRevisionRequest
{
    /// <summary>
    /// Revision request comment (required - supervisor needs to know what to fix).
    /// </summary>
    public string Comment { get; init; } = string.Empty;
}