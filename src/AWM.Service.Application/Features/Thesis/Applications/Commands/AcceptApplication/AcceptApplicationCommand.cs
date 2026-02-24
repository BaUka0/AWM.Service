namespace AWM.Service.Application.Features.Thesis.Applications.Commands.AcceptApplication;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a supervisor to accept a student's application to a topic.
/// </summary>
public sealed record AcceptApplicationCommand : IRequest<Result>
{
    /// <summary>
    /// ID of the application to accept.
    /// </summary>
    public long ApplicationId { get; init; }
}