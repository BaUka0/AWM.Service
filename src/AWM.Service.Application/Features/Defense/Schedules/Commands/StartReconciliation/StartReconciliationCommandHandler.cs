using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.StartReconciliation;

public sealed class StartReconciliationCommandHandler : IRequestHandler<StartReconciliationCommand, Result>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public StartReconciliationCommandHandler(
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository;
        _commissionRepository = commissionRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(StartReconciliationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule == null)
        {
            return Result.Failure(new Error("Schedule.NotFound", $"Schedule with ID {request.ScheduleId} not found."));
        }
        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(schedule.CommissionId, cancellationToken);

        if (commission == null)
        {
            return Result.Failure(new Error("Commission.NotFound", "Commission for this schedule not found."));
        }

        // Check if user is Secretary
        var userAssignment = commission.Assignments.FirstOrDefault(a => a.UserId == currentUserId && a.IsActive && !a.IsDeleted);
        if (userAssignment == null || userAssignment.RoleType != StaffRoleType.CommissionSecretary)
        {
            return Result.Failure(new Error("Commission.Unauthorized", "Only the technical secretary of the commission can start reconciliation."));
        }

        try
        {
            schedule.StartReconciliation(currentUserId);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(new Error(ex.ErrorCode, ex.Message));
        }
    }
}
