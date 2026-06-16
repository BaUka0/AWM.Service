using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.WithdrawApplication;

public record WithdrawApplicationCommand(long ApplicationId) : IRequest<Result>;
