using AWM.Service.Application.Features.Workflow.Stages.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.SetStagesPeriods;

public sealed class SetStagesPeriodsCommandHandler : IRequestHandler<SetStagesPeriodsCommand, Result<Unit>>
{
    private readonly IStageRepository _stageRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IStudentReadOnlyRepository _studentRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SetStagesPeriodsCommandHandler(
        IStageRepository stageRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        IStudentReadOnlyRepository studentRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _stageRepository = stageRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _studentRepository = studentRepository;
        _employeeRepository = employeeRepository;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(SetStagesPeriodsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        // 1. Determine OrgUnitId (Department)
        int orgUnitId;
        if (request.OrgUnitId.HasValue)
        {
            orgUnitId = request.OrgUnitId.Value;
        }
        else
        {
            var employee = await _employeeRepository.GetByUserIdAsync(currentUserId, cancellationToken);
            if (employee == null)
            {
                return Result.Failure<Unit>(new Error("Stages.EmployeeNotFound", "Employee record not found for the current user in University SoT."));
            }

            var mainPosition = employee.Positions.FirstOrDefault(p => p.IsMainPosition) 
                               ?? employee.Positions.FirstOrDefault();
            
            if (mainPosition == null)
            {
                return Result.Failure<Unit>(new Error("Stages.OrgUnitNotFound", "Employee has no assigned department in University SoT."));
            }

            orgUnitId = mainPosition.OrgUnitId;
        }
        
        // 2. Fetch existing stages for the department and semester
        var existingStages = await _stageRepository.GetByDepartmentAsync(
            orgUnitId, 
            request.SemesterId, 
            cancellationToken);

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

        // 3. Notifications
        // For Supervisors (Teachers)
        var supervisors = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit", 
            orgUnitId, 
            Domain.CommonDomain.Enums.StaffRoleType.Supervisor, 
            cancellationToken);

        var supervisorUserIds = supervisors.Select(s => s.UserId).Distinct().ToList();
        if (supervisorUserIds.Any())
        {
            await _notificationService.SendToManyAsync(
                supervisorUserIds,
                "Утверждены периоды подачи направлений и тем",
                currentUserId,
                "Кафедра утвердила сроки подачи направлений и тем. Пожалуйста, ознакомьтесь с ними в системе.",
                null,
                "OrgUnit",
                orgUnitId,
                cancellationToken);
        }

        // For Students
        if (request.SpecialityId.HasValue)
        {
            var students = await _studentRepository.GetBySpecialityAsync(request.SpecialityId.Value, cancellationToken);
            var studentUserIds = students.Select(s => s.Id).ToList();
            if (studentUserIds.Any())
            {
                await _notificationService.SendToManyAsync(
                    studentUserIds,
                    "Утверждены сроки выбора тем",
                    currentUserId,
                    "Утверждены сроки выбора тем. Вы сможете выбрать тему в установленный период.",
                    null,
                    "OrgUnit",
                    orgUnitId,
                    cancellationToken);
            }
        }

        return Result.Success(Unit.Value);
    }
}
