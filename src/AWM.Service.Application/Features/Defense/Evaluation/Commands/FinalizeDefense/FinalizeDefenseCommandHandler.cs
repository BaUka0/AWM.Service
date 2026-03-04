namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.FinalizeDefense;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for finalizing a defense.
/// Finds the protocol by schedule ID, finalizes it, computes the final grade,
/// marks the student work as defended, and notifies the student.
/// </summary>
public sealed class FinalizeDefenseCommandHandler : IRequestHandler<FinalizeDefenseCommand, Result>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IStudentWorkRepository _workRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public FinalizeDefenseCommandHandler(
        IProtocolRepository protocolRepository,
        IScheduleRepository scheduleRepository,
        IStudentWorkRepository workRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _protocolRepository = protocolRepository ?? throw new ArgumentNullException(nameof(protocolRepository));
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(FinalizeDefenseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var protocol = await _protocolRepository.GetByScheduleIdAsync(request.ScheduleId, cancellationToken);
            if (protocol is null)
                return Result.Failure(new Error("NotFound.Protocol",
                    $"Protocol for schedule {request.ScheduleId} not found. Generate a protocol first."));

            protocol.Finalize(userId.Value);
            await _protocolRepository.UpdateAsync(protocol, cancellationToken);

            // Compute final grade from schedule average score
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule is not null && schedule.WorkId > 0)
            {
                var averageScore = schedule.GetAverageScore();
                var finalGrade = MapScoreToGrade(averageScore);

                var work = await _workRepository.GetByIdWithDetailsAsync(schedule.WorkId, cancellationToken);
                if (work is not null)
                {
                    work.MarkAsDefended(finalGrade);
                    await _workRepository.UpdateAsync(work, cancellationToken);

                    // Notify participants about defense result
                    var studentUserIds = new List<int>();
                    foreach (var participant in work.Participants)
                    {
                        var student = await _studentRepository.GetByIdAsync(
                            participant.StudentId, cancellationToken);
                        if (student is not null)
                            studentUserIds.Add(student.UserId);
                    }

                    if (studentUserIds.Count > 0)
                    {
                        var body = finalGrade is not null
                            ? $"Итоговая оценка: {finalGrade}."
                            : "Защита завершена.";

                        await _notificationService.SendToManyAsync(
                            studentUserIds,
                            "Результат защиты",
                            userId.Value,
                            body,
                            relatedEntityType: "StudentWork",
                            relatedEntityId: work.Id,
                            cancellationToken: cancellationToken);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ioEx)
        {
            return Result.Failure(new Error("BusinessRule.Protocol", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }

    private static string? MapScoreToGrade(decimal? averageScore)
    {
        if (!averageScore.HasValue) return null;

        return averageScore.Value switch
        {
            >= 90 => "Отлично",
            >= 75 => "Хорошо",
            >= 50 => "Удовлетворительно",
            _ => "Неудовлетворительно"
        };
    }
}
