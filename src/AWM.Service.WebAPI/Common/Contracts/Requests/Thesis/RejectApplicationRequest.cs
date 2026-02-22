namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for rejecting a topic application.
/// </summary>
public sealed record RejectApplicationRequest
{
    /// <summary>
    /// Reason for rejecting the application.
    /// </summary>
    /// <example>The student does not meet the prerequisites for this topic.</example>
    public string RejectReason { get; init; } = null!;
}
