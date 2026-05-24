using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.RejectApplication;

public record RejectApplicationCommand(long ApplicationId, string? Reason = null) : IRequest<Result>;
