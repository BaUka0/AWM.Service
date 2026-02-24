namespace AWM.Service.Application.Features.Thesis.Applications.Commands.RejectApplication;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a supervisor to reject a student's application to a topic.
/// </summary>
public sealed record RejectApplicationCommand : IRequest<Result>
{
    /// <summary>
    /// ID of the application to reject.
    /// </summary>
    public long ApplicationId { get; init; }

    /// <summary>
    /// Optional comment explaining the rejection reason.
    /// </summary>
    public string? RejectReason { get; init; }
}