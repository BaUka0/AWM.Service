using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.AdmitToDefense;

public sealed record AdmitToDefenseCommand(long WorkId) : IRequest<Result>;
