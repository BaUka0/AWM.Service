namespace AWM.Service.Application.Features.Thesis.Topics.Commands.UpdateTopic;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update an existing thesis topic.
/// Only allowed in draft state (before approval).
/// </summary>
public sealed record UpdateTopicCommand : IRequest<Result>
{
    /// <summary>
    /// Topic ID to update.
    /// </summary>
    public long TopicId { get; init; }

    /// <summary>
    /// Updated topic title in Russian.
    /// </summary>
    public string TitleRu { get; init; } = null!;

    /// <summary>
    /// Updated topic title in Kazakh (optional).
    /// </summary>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Updated topic title in English (optional).
    /// </summary>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Updated topic description (optional).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Updated maximum number of participants (1-5).
    /// </summary>
    public int MaxParticipants { get; init; }

    /// <summary>
    /// ID of the user modifying the topic (for audit).
    /// </summary>
    public int ModifiedBy { get; init; }
}