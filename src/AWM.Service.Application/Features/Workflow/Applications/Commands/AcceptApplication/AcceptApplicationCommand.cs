using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.AcceptApplication;

public record AcceptApplicationCommand(long ApplicationId) : IRequest<Result>;
