namespace AWM.Service.Application.Features.Thesis.Topics.Commands.BulkApproveTopics;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for bulk approval of topics by the department.
/// Accepts a list of topic IDs to approve at once.
/// </summary>
public sealed record BulkApproveTopicsCommand : IRequest<Result>
{
    /// <summary>
    /// List of topic IDs to approve.
    /// </summary>
    public IReadOnlyList<long> TopicIds { get; init; } = [];
}
