namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

/// <summary>
/// Request to send topics back to supervisors for revision.
/// </summary>
public record SendTopicsBackForRevisionRequest(IReadOnlyList<long> TopicIds, string Comment);
