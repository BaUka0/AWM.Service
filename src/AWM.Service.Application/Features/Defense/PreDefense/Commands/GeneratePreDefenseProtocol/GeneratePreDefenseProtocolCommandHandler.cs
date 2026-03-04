namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseProtocol;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for generating a pre-defense session protocol (ведомость предзащиты).
/// Creates a Protocol entity for the entire commission session on a given date.
/// </summary>
public sealed class GeneratePreDefenseProtocolCommandHandler
    : IRequestHandler<GeneratePreDefenseProtocolCommand, Result<long>>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GeneratePreDefenseProtocolCommandHandler> _logger;

    public GeneratePreDefenseProtocolCommandHandler(
        IProtocolRepository protocolRepository,
        ICommissionRepository commissionRepository,
        IScheduleRepository scheduleRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<GeneratePreDefenseProtocolCommandHandler> logger)
    {
        _protocolRepository = protocolRepository ?? throw new ArgumentNullException(nameof(protocolRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<long>> Handle(GeneratePreDefenseProtocolCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<long>(new Error("401", "User ID is not available."));

            var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
            if (commission is null)
                return Result.Failure<long>(new Error("NotFound.Commission",
                    $"Commission with ID {request.CommissionId} not found."));

            if (commission.CommissionType != CommissionType.PreDefense)
                return Result.Failure<long>(new Error("BusinessRule.Commission",
                    "The specified commission is not a PreDefense commission."));

            // Check if protocol already exists for this commission session
            var existingProtocols = await _protocolRepository.GetByCommissionAsync(
                request.CommissionId, cancellationToken);
            var duplicateProtocol = existingProtocols
                .FirstOrDefault(p => p.SessionDate.Date == request.SessionDate.Date);
            if (duplicateProtocol is not null)
                return Result.Failure<long>(new Error("BusinessRule.Protocol",
                    $"A protocol already exists for this commission on {request.SessionDate:dd.MM.yyyy}."));

            // Verify there are scheduled works for this commission
            var schedules = await _scheduleRepository.GetByCommissionAsync(
                request.CommissionId, cancellationToken);
            if (!schedules.Any(s => !s.IsDeleted))
                return Result.Failure<long>(new Error("BusinessRule.Protocol",
                    "No scheduled works found for this commission."));

            // Use the first schedule as the anchor (protocol covers the whole session)
            var firstSchedule = schedules
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.DefenseDate)
                .First();

            var protocol = new Protocol(
                firstSchedule.Id,
                request.CommissionId,
                request.SessionDate,
                userId.Value);

            await _protocolRepository.AddAsync(protocol, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Generated pre-defense protocol for Commission={CommissionId}, Date={Date}",
                request.CommissionId, request.SessionDate);

            return Result.Success(protocol.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GeneratePreDefenseProtocol failed for Commission={CommissionId}",
                request.CommissionId);
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
