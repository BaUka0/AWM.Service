namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for creating a topic application.
/// </summary>
public sealed record CreateApplicationRequest
{
    /// <summary>
    /// ID of the topic to apply to.
    /// </summary>
    /// <example>1</example>
    public long TopicId { get; init; }

    /// <summary>
    /// Optional motivation letter from the student.
    /// </summary>
    /// <example>I am interested in this topic because...</example>
    public string? MotivationLetter { get; init; }
}
