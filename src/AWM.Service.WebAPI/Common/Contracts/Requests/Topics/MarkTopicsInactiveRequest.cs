namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

/// <summary>
/// Request to mark selected topics as inactive (no students applied).
/// </summary>
public record MarkTopicsInactiveRequest(IReadOnlyList<long> TopicIds);
