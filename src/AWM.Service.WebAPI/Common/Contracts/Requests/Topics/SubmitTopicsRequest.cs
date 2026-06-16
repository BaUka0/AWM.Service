namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

/// <summary>
/// Request to submit a batch of topics for department review.
/// </summary>
public record SubmitTopicsRequest(List<long> TopicIds);
