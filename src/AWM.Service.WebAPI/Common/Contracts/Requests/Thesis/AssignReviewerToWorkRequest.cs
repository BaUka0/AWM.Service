namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for assigning a reviewer to a student work.
/// </summary>
public sealed record AssignReviewerToWorkRequest
{
    /// <summary>
    /// Reviewer ID to assign.
    /// </summary>
    /// <example>5</example>
    public int ReviewerId { get; init; }
}
