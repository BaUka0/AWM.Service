using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.CreateApplication;

public record CreateApplicationCommand(
    long TopicId,
    string? MotivationLetter = null) : IRequest<Result<long>>;
