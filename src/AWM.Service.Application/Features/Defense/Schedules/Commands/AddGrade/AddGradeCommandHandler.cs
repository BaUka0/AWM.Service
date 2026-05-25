using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.AddGrade;

public sealed class AddGradeCommandHandler : IRequestHandler<AddGradeCommand, Result<long>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AddGradeCommandHandler(
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

    public async Task<Result<long>> Handle(AddGradeCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure<long>(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule == null)
        {
            return Result.Failure<long>(new Error("Schedule.NotFound", $"Schedule with ID {request.ScheduleId} not found."));
        }
        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(schedule.CommissionId, cancellationToken);
        
        if (commission == null)
        {
            return Result.Failure<long>(new Error("Commission.NotFound", "Commission for this schedule not found."));
        }

        var userAssignment = commission.Assignments.FirstOrDefault(a => a.UserId == currentUserId && a.IsActive && !a.IsDeleted);
        if (userAssignment == null)
        {
            return Result.Failure<long>(new Error("Commission.NotAMember", "Current user is not a member of the commission for this schedule."));
        }

        try
        {
            var grade = schedule.AddGrade(userAssignment.Id, request.CriteriaId, request.Score, currentUserId, request.Comment);
            
            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(grade.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure<long>(new Error(ex.ErrorCode, ex.Message));
        }
    }
}
