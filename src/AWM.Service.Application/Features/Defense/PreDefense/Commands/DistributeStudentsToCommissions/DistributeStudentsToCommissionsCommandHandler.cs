namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.DistributeStudentsToCommissions;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class DistributeStudentsToCommissionsCommandHandler
    : IRequestHandler<DistributeStudentsToCommissionsCommand, Result<int>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IPeriodValidationService _periodValidationService;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DistributeStudentsToCommissionsCommandHandler> _logger;

    public DistributeStudentsToCommissionsCommandHandler(
        IStudentWorkRepository workRepository,
        ICommissionRepository commissionRepository,
        IScheduleRepository scheduleRepository,
        IPreDefenseAttemptRepository attemptRepository,
        IPeriodValidationService periodValidationService,
        INotificationService notificationService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<DistributeStudentsToCommissionsCommandHandler> logger)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _periodValidationService = periodValidationService ?? throw new ArgumentNullException(nameof(periodValidationService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(DistributeStudentsToCommissionsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<int>(new Error("401", "User ID is not available."));

            // Validate pre-defense period is open
            var stage = request.PreDefenseNumber switch
            {
                1 => WorkflowStage.PreDefense1,
                2 => WorkflowStage.PreDefense2,
                3 => WorkflowStage.PreDefense3,
                _ => WorkflowStage.PreDefense1
            };

            var (isAllowed, errorMessage) = await _periodValidationService.ValidateOperationInPeriodAsync(
                request.DepartmentId, request.AcademicYearId, stage, cancellationToken);
            if (!isAllowed)
                return Result.Failure<int>(new Error("400", errorMessage!));

            // Get all PreDefense commissions for this round
            var commissions = await _commissionRepository.GetByTypeAsync(
                request.DepartmentId, request.AcademicYearId, CommissionType.PreDefense, cancellationToken);
            var targetCommissions = commissions
                .Where(c => c.PreDefenseNumber == request.PreDefenseNumber)
                .ToList();

            if (!targetCommissions.Any())
                return Result.Failure<int>(new Error("404",
                    $"No PreDefense commissions found for round {request.PreDefenseNumber}."));

            // Get all student works in department
            var allWorks = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);

            // Filter: only works not yet scheduled for this pre-defense round
            var worksToDistribute = new List<Domain.Thesis.Entities.StudentWork>();
            foreach (var work in allWorks)
            {
                var attempts = await _attemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                var hasAttemptForRound = attempts.Any(a => a.PreDefenseNumber == request.PreDefenseNumber);
                if (!hasAttemptForRound)
                    worksToDistribute.Add(work);
            }

            if (!worksToDistribute.Any())
                return Result.Success(0);

            _logger.LogInformation("Distributing {WorkCount} works across {CommissionCount} commissions for PreDefense {Round}",
                worksToDistribute.Count, targetCommissions.Count, request.PreDefenseNumber);

            // Round-robin distribution
            var distributed = 0;
            var defaultDate = DateTime.UtcNow.AddDays(7); // Placeholder date; GeneratePreDefenseSlots will set exact times

            for (var i = 0; i < worksToDistribute.Count; i++)
            {
                var work = worksToDistribute[i];
                var commission = targetCommissions[i % targetCommissions.Count];

                var schedule = new Schedule(
                    commission.Id,
                    work.Id,
                    defaultDate,
                    userId.Value);

                await _scheduleRepository.AddAsync(schedule, cancellationToken);

                var attempt = new PreDefenseAttempt(
                    work.Id,
                    request.PreDefenseNumber,
                    userId.Value,
                    scheduleId: schedule.Id);

                await _attemptRepository.AddAsync(attempt, cancellationToken);
                distributed++;
            }

            // Notify students about their assigned commission
            var studentUserIds = worksToDistribute
                .SelectMany(w => w.Participants.Select(p => p.StudentId))
                .Distinct()
                .ToList();

            if (studentUserIds.Any())
            {
                await _notificationService.SendToManyAsync(
                    studentUserIds,
                    "Распределение на предзащиту",
                    userId.Value,
                    $"Вы распределены на предзащиту №{request.PreDefenseNumber}. Ожидайте расписание.",
                    cancellationToken: cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(distributed);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DistributeStudents failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}
