using AWM.Service.Application.Features.Workflow.Stages.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.ApproveChecksPeriods;

public record ApproveChecksPeriodsCommand(
    int SemesterId,
    int? OrgUnitId,
    int? SpecialityId,
    IReadOnlyList<StagePeriodDto> Periods) : IRequest<Result<Unit>>;

public sealed class ApproveChecksPeriodsCommandHandler : IRequestHandler<ApproveChecksPeriodsCommand, Result<Unit>>
{
    private readonly IStageRepository _stageRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;

    public ApproveChecksPeriodsCommandHandler(
        IStageRepository stageRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        IEmployeeReadOnlyRepository employeeRepository)
    {
        _stageRepository = stageRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _employeeRepository = employeeRepository;
    }

    public async Task<Result<Unit>> Handle(ApproveChecksPeriodsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        int orgUnitId;
        if (request.OrgUnitId.HasValue)
        {
            orgUnitId = request.OrgUnitId.Value;
        }
        else
        {
            var employee = await _employeeRepository.GetByUserIdAsync(currentUserId, cancellationToken);
            var mainPosition = employee?.Positions.FirstOrDefault(p => p.IsMainPosition) ?? employee?.Positions.FirstOrDefault();
            if (mainPosition == null)
                return Result.Failure<Unit>(new Error("Stages.OrgUnitNotFound", "Employee has no assigned department."));
            orgUnitId = mainPosition.OrgUnitId;
        }

        var existingStages = await _stageRepository.GetByOrgUnitAsync(orgUnitId, request.SemesterId, cancellationToken);

        foreach (var periodDto in request.Periods)
        {
            var existingStage = existingStages.FirstOrDefault(s => 
                s.WorkflowStageId == periodDto.WorkflowStageId && 
                s.SpecialityId == request.SpecialityId);

            if (existingStage != null)
            {
                existingStage.UpdateDates(periodDto.StartDate, periodDto.EndDate, currentUserId);
                await _stageRepository.UpdateAsync(existingStage, cancellationToken);
            }
            else
            {
                var newStage = new Stage(
                    orgUnitId,
                    request.SemesterId,
                    periodDto.WorkflowStageId,
                    periodDto.StartDate,
                    periodDto.EndDate,
                    currentUserId,
                    request.SpecialityId);
                
                await _stageRepository.AddAsync(newStage, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify Quality Experts
        var experts = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit", 
            orgUnitId, 
            Domain.CommonDomain.Enums.StaffRoleType.QualityExpert, 
            cancellationToken);

        var expertUserIds = experts.Where(e => e.IsActive && !e.IsDeleted).Select(e => e.UserId).Distinct().ToList();

        if (expertUserIds.Any())
        {
            await _notificationService.SendToManyAsync(
                expertUserIds,
                "Утверждены периоды нормоконтроля и проверок",
                currentUserId,
                "Периоды проведения нормоконтроля и проверок на антиплагиат были утверждены.",
                null,
                "OrgUnit",
                orgUnitId,
                cancellationToken);
        }

        return Result.Success(Unit.Value);
    }
}
