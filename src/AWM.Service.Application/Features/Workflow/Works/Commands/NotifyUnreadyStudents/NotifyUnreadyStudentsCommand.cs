using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.NotifyUnreadyStudents;

/// <summary>
/// Command to send notifications to students who are not admitted to defense.
/// </summary>
public sealed record NotifyUnreadyStudentsCommand(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null) : IRequest<Result>;
