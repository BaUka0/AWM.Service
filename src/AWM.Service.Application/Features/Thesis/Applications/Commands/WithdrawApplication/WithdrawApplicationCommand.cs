namespace AWM.Service.Application.Features.Thesis.Applications.Commands.WithdrawApplication;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a student to withdraw their application to a topic.
/// </summary>
public sealed record WithdrawApplicationCommand : IRequest<Result>
{
    /// <summary>
    /// ID of the application to withdraw.
    /// </summary>
    public long ApplicationId { get; init; }
}