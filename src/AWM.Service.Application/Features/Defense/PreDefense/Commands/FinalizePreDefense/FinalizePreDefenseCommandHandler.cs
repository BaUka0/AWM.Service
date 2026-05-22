namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.FinalizePreDefense;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.Wf.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for finalizing a pre-defense attempt.
/// Records the average score and pass/fail result on the domain entity,
/// and automatically transitions the work to the next state.
/// </summary>
public sealed class FinalizePreDefenseCommandHandler : IRequestHandler<FinalizePreDefenseCommand, Result>
{
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IStudentWorkRepository _workRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStateMachine _stateMachine;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public FinalizePreDefenseCommandHandler(
        IPreDefenseAttemptRepository attemptRepository,
        IStudentWorkRepository workRepository,
        ITopicRepository topicRepository,
        IStateMachine stateMachine,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(FinalizePreDefenseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken);
            if (attempt is null)
                return Result.Failure(new Error("NotFound.Attempt",
                    $"PreDefenseAttempt with ID {request.AttemptId} not found."));

            // RecordResult enforces that attendance must be Attended
            attempt.RecordResult(request.AverageScore, request.IsPassed, userId.Value);
            await _attemptRepository.UpdateAsync(attempt, cancellationToken);

            // Automate state transition for StudentWork
            var work = await _workRepository.GetByIdAsync(attempt.WorkId, cancellationToken);
            if (work != null && work.TopicId.HasValue)
            {
                var topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
                if (topic != null)
                {
                    var targetStateName = DetermineTargetState(attempt.PreDefenseNumber, request.IsPassed);
                    var targetState = await _stateMachine.GetStateByNameAsync(targetStateName, topic.WorkTypeId, cancellationToken);
                    
                    if (targetState != null)
                    {
                        work.ChangeState(targetState.Id, userId.Value, 
                            $"Автоматический переход после предзащиты №{attempt.PreDefenseNumber} (Результат: {(request.IsPassed ? "Сдано" : "Не сдано")})");
                        await _workRepository.UpdateAsync(work, cancellationToken);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ioEx)
        {
            return Result.Failure(new Error("BusinessRule.PreDefense", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }

    private static string DetermineTargetState(int round, bool isPassed)
    {
        return round switch
        {
            1 => WorkStates.PreDefense2WaitingForFiles,
            2 => isPassed ? WorkStates.ChecksWaitingForInitial : WorkStates.PreDefense3WaitingForFiles,
            3 => isPassed ? WorkStates.ChecksWaitingForInitial : WorkStates.Cancelled,
            _ => WorkStates.Cancelled
        };
    }
}
