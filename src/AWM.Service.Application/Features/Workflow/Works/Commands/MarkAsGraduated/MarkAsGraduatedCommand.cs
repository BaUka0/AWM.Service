using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.MarkAsGraduated;

public record MarkAsGraduatedCommand(IReadOnlyList<long> WorkIds) : IRequest<Result<Unit>>;

public sealed class MarkAsGraduatedCommandHandler : IRequestHandler<MarkAsGraduatedCommand, Result<Unit>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;

    public MarkAsGraduatedCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        IStaffAssignmentRepository staffAssignmentRepository)
    {
        _studentWorkRepository = studentWorkRepository;
        _currentUserProvider = currentUserProvider;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
        _staffAssignmentRepository = staffAssignmentRepository;
    }

    public async Task<Result<Unit>> Handle(MarkAsGraduatedCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        // Verify the user has Secretary or Chairman role
        var assignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);
        bool hasAccess = assignments.Any(a => a.IsActive && !a.IsDeleted &&
            (a.RoleType == StaffRoleType.CommissionSecretary || a.RoleType == StaffRoleType.CommissionChairman));

        if (!hasAccess)
        {
            return Result.Failure<Unit>(new Error("Forbidden", "Only Commission Secretary or Chairman can graduate students."));
        }

        if (request.WorkIds == null || !request.WorkIds.Any())
        {
            return Result.Success(Unit.Value);
        }

        var works = await _studentWorkRepository.GetByIdsAsync(request.WorkIds, cancellationToken);

        foreach (var work in works)
        {
            var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
            if (currentState == null)
            {
                continue;
            }

            // Must be defended to be graduated
            if (currentState.SystemName != WorkStates.Defended)
            {
                continue;
            }

            var targetState = await _workflowRepository.GetStateBySystemNameAsync(currentState.WorkTypeId, WorkStates.Graduated, cancellationToken);
            if (targetState != null)
            {
                work.MarkAsGraduated(work.FinalGrade);
                work.ChangeState(targetState.Id, currentUserId, "Переведен в статус 'Выпускник'");
                await _studentWorkRepository.UpdateAsync(work, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}
