namespace AWM.Service.Application.Features.Common.Stages.Commands.ApproveInitialStages;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.CommonDomain.Services;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class ApproveInitialStagesCommandHandler : IRequestHandler<ApproveInitialStagesCommand, Result>
{
    private readonly IStageRepository _stageRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IAcademicProgramRepository _academicProgramRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveInitialStagesCommandHandler> _logger;

    public ApproveInitialStagesCommandHandler(
        IStageRepository stageRepository,
        IStaffRepository staffRepository,
        IAcademicProgramRepository academicProgramRepository,
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveInitialStagesCommandHandler> logger)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _academicProgramRepository = academicProgramRepository ?? throw new ArgumentNullException(nameof(academicProgramRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(ApproveInitialStagesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("Attempting to ApproveInitialStages for Dept={DeptId}, Year={YearId} by User={UserId}",
                request.DepartmentId, request.SemesterId, userId);

            if (!userId.HasValue)
            {
                _logger.LogWarning("ApproveInitialStages failed: User ID is not available.");
                return Result.Failure(new Error("401", "User ID is not available."));
            }

            var existingStages = await _stageRepository.GetTrackedByDepartmentAsync(request.DepartmentId, request.SemesterId, cancellationToken);
            var activeStages = existingStages.Where(p => !p.IsDeleted).ToList();

            // Group by WorkflowStageId to handle potential duplicates in the request payload
            var groupedRequestedStages = request.Stages
                .GroupBy(p => p.WorkflowStageId)
                .Select(g => g.First())
                .ToList();

            foreach (var requestedStage in groupedRequestedStages)
            {
                var existing = activeStages.FirstOrDefault(p => p.WorkflowStageId == requestedStage.WorkflowStageId);
                if (existing != null)
                {
                    existing.UpdateDates(requestedStage.StartDate, requestedStage.EndDate, userId.Value);
                    await _stageRepository.UpdateAsync(existing, cancellationToken);
                }
                else
                {
                    _logger.LogDebug("Creating new stage for Stage={Stage} in Dept={DeptId}", requestedStage.WorkflowStageId, request.DepartmentId);
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
            _logger.LogInformation("Processed {StageCount} stages for Dept={DeptId}", groupedRequestedStages.Count, request.DepartmentId);

            // 1. Notify Supervisors about Direction Submission
            var directionStage = request.Stages.FirstOrDefault(p => p.WorkflowStageId == 1);
            if (directionStage != null)
            {
                var supervisors = await _staffRepository.GetSupervisorsWithCapacityAsync(request.DepartmentId, cancellationToken);
                var supervisorUserIds = supervisors.Select(s => s.UserId).ToList();

                if (supervisorUserIds.Any())
                {
                    var title = "Начало периода формирования направлений";
                    var body = $"Период формирования направлений и тем утвержден. Сроки: {directionStage.StartDate:dd.MM.yyyy} - {directionStage.EndDate:dd.MM.yyyy}. Желательно сформировать направления и темы в срок.";

                    _logger.LogInformation("Sending DirectionSubmission notifications to {SupervisorCount} supervisors", supervisorUserIds.Count);
                    await _notificationService.SendToManyAsync(
                        supervisorUserIds,
                        title,
                        userId.Value,
                        body,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    _logger.LogWarning("No supervisors found for Dept={DeptId} to notify about DirectionSubmission", request.DepartmentId);
                }
            }

            // 2. Notify Students about Topic Selection
            var selectionStage = request.Stages.FirstOrDefault(p => p.WorkflowStageId == 3);
            if (selectionStage != null)
            {
                var programs = await _academicProgramRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
                var students = await _studentRepository.GetByProgramIdsAsync(
                    programs.Select(p => p.Id).Distinct(),
                    cancellationToken);
                var studentUserIds = students
                    .Select(s => s.UserId)
                    .Distinct()
                    .ToList();

                if (studentUserIds.Any())
                {
                    var title = "Сроки выбора тем дипломных работ";
                    var body = $"Внимание! Выбор тем дипломных будет проходить в период: {selectionStage.StartDate:dd.MM.yyyy} - {selectionStage.EndDate:dd.MM.yyyy}. Пожалуйста, осуществите выбор вовремя, иначе тема будет назначена случайным образом.";

                    _logger.LogInformation("Sending TopicSelection notifications to {StudentCount} students", studentUserIds.Count);
                    await _notificationService.SendToManyAsync(
                        studentUserIds,
                        title,
                        userId.Value,
                        body,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    _logger.LogWarning("No students found for Dept={DeptId} to notify about TopicSelection", request.DepartmentId);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "ApproveInitialStages validation failed: {Message}", argEx.Message);
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ApproveInitialStages failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure(new Error("500", $"An error occurred while approving the Stages: {ex.Message}"));
        }
    }
}
