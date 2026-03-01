namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request for batch submission of topics for department approval.
/// </summary>
public sealed record SubmitTopicsForApprovalRequest
{
    /// <summary>
    /// List of topic IDs to submit.
    /// </summary>
    public IReadOnlyList<long> TopicIds { get; init; } = [];
}
