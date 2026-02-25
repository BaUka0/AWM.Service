namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract enveloping both supervisor review and external reviews for a work.
/// </summary>
public sealed record WorkReviewsResponse
{
    /// <summary>
    /// The supervisor's review (if submitted).
    /// </summary>
    public SupervisorReviewResponse? SupervisorReview { get; init; }

    /// <summary>
    /// List of external reviews assigned to the work.
    /// </summary>
    public IReadOnlyList<ReviewResponse> Reviews { get; init; } = Array.Empty<ReviewResponse>();
}
