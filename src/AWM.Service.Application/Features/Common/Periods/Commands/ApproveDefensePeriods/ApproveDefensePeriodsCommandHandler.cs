namespace AWM.Service.Application.Features.Common.Periods.Commands.ApproveDefensePeriods;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class ApproveDefensePeriodsCommandHandler : IRequestHandler<ApproveDefensePeriodsCommand, Result>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveDefensePeriodsCommandHandler> _logger;

    public ApproveDefensePeriodsCommandHandler(
        IPeriodRepository periodRepository,
        IAcademicYearRepository academicYearRepository,
        ICommissionRepository commissionRepository,
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveDefensePeriodsCommandHandler> logger)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
        _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _studentWorkRepository = studentWorkRepository ?? throw new ArgumentNullException(nameof(studentWorkRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private static readonly HashSet<WorkflowStage> DefenseStages = new()
    {
        WorkflowStage.PreDefense1,
        WorkflowStage.PreDefense2,
        WorkflowStage.PreDefense3,
        WorkflowStage.FinalDefense
    };

    public async Task<Result> Handle(ApproveDefensePeriodsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("ApproveDefensePeriods for Dept={DeptId}, Year={YearId} by User={UserId}",
                request.DepartmentId, request.AcademicYearId, userId);

            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var academicYear = await _academicYearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken);
            if (academicYear is null)
                return Result.Failure(new Error("404", $"Academic year with ID {request.AcademicYearId} not found."));

            // Validate that only defense stages are submitted
            var invalidStages = request.Periods
                .Where(p => !DefenseStages.Contains(p.WorkflowStage))
                .Select(p => p.WorkflowStage)
                .ToList();

            if (invalidStages.Any())
                return Result.Failure(new Error("400",
                    $"Invalid stages for defense period approval: {string.Join(", ", invalidStages)}. Only PreDefense1/2/3 and FinalDefense are allowed."));

            var existingPeriods = await _periodRepository.GetTrackedByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);
            var activePeriods = existingPeriods.Where(p => !p.IsDeleted).ToList();

            var groupedPeriods = request.Periods
                .GroupBy(p => p.WorkflowStage)
                .Select(g => g.First())
                .ToList();

            foreach (var requestedPeriod in groupedPeriods)
            {
                var existing = activePeriods.FirstOrDefault(p => p.WorkflowStage == requestedPeriod.WorkflowStage);
                if (existing != null)
                {
                    existing.UpdateDates(requestedPeriod.StartDate, requestedPeriod.EndDate, userId.Value);
                    await _periodRepository.UpdateAsync(existing, cancellationToken);
                }
                else
                {
                    var newPeriod = new Period(
                        request.DepartmentId,
                        request.AcademicYearId,
                        requestedPeriod.WorkflowStage,
                        requestedPeriod.StartDate,
                        requestedPeriod.EndDate,
                        userId.Value);
                    await _periodRepository.AddAsync(newPeriod, cancellationToken);
                }
            }

            // Notify students about pre-defense schedule
            var preDefensePeriod = request.Periods.FirstOrDefault(p => p.WorkflowStage == WorkflowStage.PreDefense1);
            if (preDefensePeriod != null)
            {
                var works = await _studentWorkRepository.GetByDepartmentAsync(
                    request.DepartmentId, request.AcademicYearId, cancellationToken);
                var studentUserIds = works
                    .SelectMany(w => w.Participants.Select(p => p.StudentId))
                    .Distinct()
                    .ToList();

                if (studentUserIds.Any())
                {
                    await _notificationService.SendToManyAsync(
                        studentUserIds,
                        "Расписание предзащит утверждено",
                        userId.Value,
                        $"Период предзащит утвержден. Первая предзащита: {preDefensePeriod.StartDate:dd.MM.yyyy} - {preDefensePeriod.EndDate:dd.MM.yyyy}.",
                        cancellationToken: cancellationToken);
                }
            }

            // Notify commission members
            var commissions = await _commissionRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);
            var memberUserIds = commissions
                .SelectMany(c => c.Members.Select(m => m.UserId))
                .Distinct()
                .ToList();

            if (memberUserIds.Any())
            {
                await _notificationService.SendToManyAsync(
                    memberUserIds,
                    "Периоды защит утверждены",
                    userId.Value,
                    "Периоды предзащит и защиты утверждены. Проверьте расписание комиссий.",
                    cancellationToken: cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "ApproveDefensePeriods validation failed: {Message}", argEx.Message);
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ApproveDefensePeriods failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
