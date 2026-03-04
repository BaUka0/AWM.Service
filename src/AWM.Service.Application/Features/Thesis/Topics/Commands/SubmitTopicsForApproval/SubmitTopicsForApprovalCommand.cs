namespace AWM.Service.Application.Features.Thesis.Topics.Commands.SubmitTopicsForApproval;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for batch submission of topics for department approval.
/// Supervisor sends multiple draft topics to the department at once.
/// </summary>
public sealed record SubmitTopicsForApprovalCommand : IRequest<Result>
{
    /// <summary>
    /// List of topic IDs to submit for approval.
    /// </summary>
    public IReadOnlyList<long> TopicIds { get; init; } = [];
}
