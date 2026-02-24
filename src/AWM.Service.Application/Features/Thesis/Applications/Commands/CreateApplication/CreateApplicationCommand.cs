namespace AWM.Service.Application.Features.Thesis.Applications.Commands.CreateApplication;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a student to apply to a topic.
/// </summary>
public sealed record CreateApplicationCommand : IRequest<Result<long>>
{
    /// <summary>
    /// ID of the topic to apply to.
    /// </summary>
    public long TopicId { get; init; }

    /// <summary>
    /// Optional motivation letter.
    /// </summary>
    public string? MotivationLetter { get; init; }
}