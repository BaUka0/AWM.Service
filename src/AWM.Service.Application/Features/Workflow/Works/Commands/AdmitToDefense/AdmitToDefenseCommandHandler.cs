using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.AdmitToDefense;

/// <summary>
/// Handler for admitting a student work to the final defense stage (GAK).
/// Transitions the work state to ReadyForDefense.
/// </summary>
public sealed class AdmitToDefenseCommandHandler : IRequestHandler<AdmitToDefenseCommand, Result>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AdmitToDefenseCommandHandler(
        IStudentWorkRepository workRepository,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    /// <summary>
    /// Handles the state transition of a work to the ReadyForDefense state.
    /// </summary>
    public async Task<Result> Handle(AdmitToDefenseCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId ?? 0;
        if (currentUserId == 0)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User not authenticated."));
        }

        var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure(new Error("Work.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
        if (currentState == null)
        {
            return Result.Failure(new Error("Work.StateNotFound", "Current state not resolved."));
        }

        // Transition to ReadyForDefense
        var targetState = await _workflowRepository.GetStateBySystemNameAsync(currentState.WorkTypeId, WorkStates.ReadyForDefense, cancellationToken);
        if (targetState == null)
        {
            return Result.Failure(new Error("Work.TargetStateNotFound", "Target state 'ReadyForDefense' not found."));
        }

        work.ChangeState(targetState.Id, currentUserId, "Formally admitted to final defense.");
        await _workRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
