namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.GenerateProtocol;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for generating a defense session protocol.
/// Creates a new Protocol entity tied to the schedule and commission.
/// </summary>
public sealed class GenerateProtocolCommandHandler : IRequestHandler<GenerateProtocolCommand, Result<long>>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GenerateProtocolCommandHandler(
        IProtocolRepository protocolRepository,
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _protocolRepository = protocolRepository ?? throw new ArgumentNullException(nameof(protocolRepository));
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(GenerateProtocolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<long>(new Error("401", "User ID is not available."));

            // Verify the schedule exists
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule is null)
                return Result.Failure<long>(new Error("NotFound.Schedule",
                    $"Schedule with ID {request.ScheduleId} not found."));

            // Check if a protocol already exists for this schedule
            var existingProtocol = await _protocolRepository.GetByScheduleIdAsync(request.ScheduleId, cancellationToken);
            if (existingProtocol is not null)
                return Result.Failure<long>(new Error("BusinessRule.Protocol",
                    $"A protocol already exists for schedule {request.ScheduleId}."));

            var protocol = new Protocol(
                request.ScheduleId,
                request.CommissionId,
                schedule.DefenseDate,
                userId.Value);

            await _protocolRepository.AddAsync(protocol, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(protocol.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
