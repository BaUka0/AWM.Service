using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.SubmitForCheck;

public record SubmitForCheckCommand(long WorkId, int CheckTypeId) : IRequest<Result<long>>;

public sealed class SubmitForCheckCommandHandler : IRequestHandler<SubmitForCheckCommand, Result<long>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitForCheckCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _currentUserProvider = currentUserProvider;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(SubmitForCheckCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<long>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<long>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        var isParticipant = work.Participants.Any(p => p.StudentId == currentUserId);
        if (!isParticipant)
        {
            return Result.Failure<long>(new Error("Checks.Forbidden", "Only participants of this work can submit it for checking."));
        }

        var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
        if (currentState == null)
        {
            return Result.Failure<long>(new Error("Workflow.StateNotFound", "Current state of work was not found."));
        }

        var hasPending = work.QualityChecks.Any(c => c.CheckTypeId == request.CheckTypeId && !c.IsPassed && !c.AssignedExpertId.HasValue);
        if (hasPending)
        {
            return Result.Failure<long>(new Error("Checks.Duplicate", "There is already a pending check request of this type."));
        }

        var hasPassed = work.QualityChecks.Any(c => c.CheckTypeId == request.CheckTypeId && c.IsPassed);
        if (hasPassed)
        {
            return Result.Failure<long>(new Error("Checks.AlreadyPassed", "This check has already been passed successfully."));
        }

        var check = work.AddQualityCheck(request.CheckTypeId, isPassed: false);

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(check.Id);
    }
}
