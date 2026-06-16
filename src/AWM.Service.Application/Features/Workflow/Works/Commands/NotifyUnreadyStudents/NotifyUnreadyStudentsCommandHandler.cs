using AWM.Service.Application.Features.Workflow.Works.Queries.GetDefenseReadiness;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.NotifyUnreadyStudents;

/// <summary>
/// Handler for sending notifications to students who are not admitted to defense.
/// </summary>
public sealed class NotifyUnreadyStudentsCommandHandler : IRequestHandler<NotifyUnreadyStudentsCommand, Result>
{
    private readonly ISender _sender;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly INotificationService _notificationService;

    public NotifyUnreadyStudentsCommandHandler(
        ISender sender,
        IStudentWorkRepository studentWorkRepository,
        INotificationService notificationService)
    {
        _sender = sender;
        _studentWorkRepository = studentWorkRepository;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(NotifyUnreadyStudentsCommand request, CancellationToken cancellationToken)
    {
        var readinessResult = await _sender.Send(
            new GetDefenseReadinessQuery(request.OrgUnitId, request.SemesterId, request.SpecialityId),
            cancellationToken);

        if (readinessResult.IsFailed)
        {
            return Result.Failure(readinessResult.Error);
        }

        var unreadyWorks = readinessResult.Value
            .Where(d => !d.Admitted)
            .ToList();

        if (!unreadyWorks.Any())
        {
            return Result.Success();
        }

        var unreadyWorkIds = unreadyWorks.Select(w => w.WorkId).ToList();
        var worksWithDetails = await _studentWorkRepository.GetByIdsWithDetailsAsync(unreadyWorkIds, cancellationToken);
        var readinessMap = unreadyWorks.ToDictionary(w => w.WorkId);

        foreach (var work in worksWithDetails)
        {
            var studentIds = work.Participants.Select(p => p.StudentId).ToList();
            if (!studentIds.Any()) continue;

            readinessMap.TryGetValue(work.Id, out var readinessDto);
            var topicTitle = readinessDto?.TopicTitle ?? "выпускная работа";

            await _notificationService.SendToManyAsync(
                userIds: studentIds,
                title: "Необходимо завершить подготовку к защите",
                createdBy: 1,
                body: $"Ваша выпускная работа по теме '{topicTitle}' на текущий момент не допущена к защите. Пожалуйста, убедитесь, что вы прошли все предзащиты, нормоконтроль, проверку на антиплагиат и получили отзыв руководителя.",
                relatedEntityType: "StudentWork",
                relatedEntityId: work.Id,
                cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
}
