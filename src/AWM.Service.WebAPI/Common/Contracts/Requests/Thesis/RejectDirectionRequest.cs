namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for rejecting a direction.
/// </summary>
public sealed record RejectDirectionRequest
{
    /// <summary>
    /// Rejection reason/comment (optional).
    /// </summary>
    public string? Comment { get; init; }
}