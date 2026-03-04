namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request for bulk approval of topics by the department.
/// </summary>
public sealed record BulkApproveTopicsRequest
{
    /// <summary>
    /// List of topic IDs to approve.
    /// </summary>
    public IReadOnlyList<long> TopicIds { get; init; } = [];
}
