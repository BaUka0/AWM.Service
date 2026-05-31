using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Commands.CreateProtocol;

public sealed class CreateProtocolCommandHandler : IRequestHandler<CreateProtocolCommand, Result<long>>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateProtocolCommandHandler(
        IProtocolRepository protocolRepository,
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _protocolRepository = protocolRepository;
        _scheduleRepository = scheduleRepository;
        _commissionRepository = commissionRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<long>> Handle(CreateProtocolCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure<long>(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule == null)
            return Result.Failure<long>(new Error("Schedule.NotFound", $"Schedule with ID {request.ScheduleId} not found."));

        // Only the commission chairman or secretary may create the protocol
        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(schedule.CommissionId, cancellationToken);
        if (commission == null)
            return Result.Failure<long>(new Error("Commission.NotFound", "Commission for this schedule not found."));

        var userAssignment = commission.Assignments
            .FirstOrDefault(a => a.UserId == currentUserId && a.IsActive && !a.IsDeleted);

        if (userAssignment == null ||
            (userAssignment.RoleType != StaffRoleType.CommissionChairman &&
             userAssignment.RoleType != StaffRoleType.CommissionSecretary))
        {
            return Result.Failure<long>(new Error("Commission.Unauthorized",
                "Only the chairman or secretary of the commission can create the protocol."));
        }

        var existingProtocol = await _protocolRepository.GetByScheduleIdAsync(request.ScheduleId, cancellationToken);
        if (existingProtocol != null)
            return Result.Failure<long>(new Error("Protocol.AlreadyExists", "Protocol for this schedule already exists."));

        try
        {
            var protocol = new Protocol(
                request.ScheduleId,
                schedule.CommissionId,
                schedule.DefenseDate,
                currentUserId,
                request.ProtocolNumber,
                request.FinalScoreNumeric,
                request.FinalGradeLetter,
                request.Decision,
                request.Comments,
                request.DecisionType,
                request.ReadinessPercent);

            await _protocolRepository.AddAsync(protocol, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(protocol.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure<long>(new Error(ex.ErrorCode, ex.Message));
        }
    }
}
