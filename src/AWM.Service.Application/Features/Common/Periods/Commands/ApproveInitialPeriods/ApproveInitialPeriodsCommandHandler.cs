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
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly ISpecialityRepository _SpecialityRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveInitialStagesCommandHandler> _logger;

    public ApproveInitialStagesCommandHandler(
        IStageRepository stageRepository,
        IEmployeeRepository EmployeeRepository,
        ISpecialityRepository SpecialityRepository,
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<ApproveInitialStagesCommandHandler> logger)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
        _EmployeeRepository = EmployeeRepository ?? throw new ArgumentNullException(nameof(EmployeeRepository));
        _SpecialityRepository = SpecialityRepository ?? throw new ArgumentNullException(nameof(SpecialityRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> Handle(ApproveInitialStagesCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("ApproveInitialStages for Dept={DeptId}, Semester={SemesterId} by User={UserId}",
            request.DepartmentId, request.SemesterId, userId);

        if (!userId.HasValue)
            return Result.Failure(new Error("401", "User ID is not available."));

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var staffList = await _EmployeeRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);
        var staffUserIds = staffList.Select(s => s.Id).Distinct().ToList();

        if (staffUserIds.Any())
        {
            await _notificationService.SendToManyAsync(
                staffUserIds,
                "Утверждены сроки начальных этапов дипломных работ",
                userId.Value,
                $"Сроки начальных этапов (выбор темы, подготовка и т.д.) для вашей кафедры на текущий семестр были успешно утверждены.",
                cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
}
