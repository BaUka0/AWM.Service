namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

/// <summary>
/// Request to reconcile (batch final-approve) selected topics.
/// </summary>
public record ReconcileTopicsRequest(IReadOnlyList<long> TopicIds);
