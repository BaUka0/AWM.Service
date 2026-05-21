namespace AWM.Service.Application.Features.Common.Stages.Commands.ApproveDefenseStages;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class ApproveDefenseStagesCommandHandler : IRequestHandler<ApproveDefenseStagesCommand, Result>
{
    private readonly IStageRepository _stageRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveDefenseStagesCommandHandler> _logger;

    public ApproveDefenseStagesCommandHandler(
        IStageRepository stageRepository,
        ICommissionRepository commissionRepository,
        IStudentWorkRepository studentWorkRepository,
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveDefenseStagesCommandHandler> logger)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _studentWorkRepository = studentWorkRepository ?? throw new ArgumentNullException(nameof(studentWorkRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private static readonly HashSet<int> DefenseStages = new()
    {
        4, // PreDefense1
        5, // PreDefense2
        6, // PreDefense3
        7  // FinalDefense
    };

    public async Task<Result> Handle(ApproveDefenseStagesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("ApproveDefenseStages for Dept={DeptId}, Year={YearId} by User={UserId}",
                request.DepartmentId, request.SemesterId, userId);

            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            // Validate that only defense stages are submitted
            var invalidStages = request.Stages
                .Where(p => !DefenseStages.Contains(p.WorkflowStageId))
                .Select(p => p.WorkflowStageId)
                .ToList();

            if (invalidStages.Any())
                return Result.Failure(new Error("400",
                    $"Invalid stages for defense stage approval: {string.Join(", ", invalidStages)}. Only PreDefense1/2/3 and FinalDefense are allowed."));

            var existingStages = await _stageRepository.GetTrackedByDepartmentAsync(
                request.DepartmentId, request.SemesterId, cancellationToken);
            var activeStages = existingStages.Where(p => !p.IsDeleted).ToList();

            var groupedStages = request.Stages
                .GroupBy(p => p.WorkflowStageId)
                .Select(g => g.First())
                .ToList();

            foreach (var requestedStage in groupedStages)
            {
                var existing = activeStages.FirstOrDefault(p => p.WorkflowStageId == requestedStage.WorkflowStageId);
                if (existing != null)
                {
                    existing.UpdateDates(requestedStage.StartDate, requestedStage.EndDate, userId.Value);
                    await _stageRepository.UpdateAsync(existing, cancellationToken);
                }
                else
                {
                    var newStage = new Stage(
                        request.DepartmentId,
                        request.SemesterId,
                        requestedStage.WorkflowStageId,
                        requestedStage.StartDate,
                        requestedStage.EndDate,
                        userId.Value);
                    await _stageRepository.AddAsync(newStage, cancellationToken);
                }
            }

            // Notify students about pre-defense schedule
            var preDefenseStage = request.Stages.FirstOrDefault(p => p.WorkflowStageId == 4);
            if (preDefenseStage != null)
            {
                var works = await _studentWorkRepository.GetByDepartmentAsync(
                    request.DepartmentId, request.SemesterId, cancellationToken);
                var students = await _studentRepository.GetByIdsAsync(
                    works
                        .SelectMany(w => w.Participants.Select(p => p.StudentId))
                        .Distinct(),
                    cancellationToken);
                var studentUserIds = students
                    .Select(s => s.Id)
                    .Distinct()
                    .ToList();

                if (studentUserIds.Any())
                {
                    await _notificationService.SendToManyAsync(
                        studentUserIds,
                        "Расписание предзащит утверждено",
                        userId.Value,
                        $"Период предзащит утвержден. Первая предзащита: {preDefenseStage.StartDate:dd.MM.yyyy} - {preDefenseStage.EndDate:dd.MM.yyyy}.",
                        cancellationToken: cancellationToken);
                }
            }

            // Notify commission members
            var commissions = await _commissionRepository.GetByDepartmentAsync(
                request.DepartmentId, request.SemesterId, cancellationToken);
            var memberUserIds = commissions
                .SelectMany(c => c.Assignments.Select(m => m.UserId))
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
            _logger.LogWarning(argEx, "ApproveDefenseStages validation failed: {Message}", argEx.Message);
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ApproveDefenseStages failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure(new Error("500", $"An error occurred: {ex.Message}"));
        }
    }
}
