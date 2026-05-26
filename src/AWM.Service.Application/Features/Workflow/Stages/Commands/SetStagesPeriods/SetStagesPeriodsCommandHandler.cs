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
    private readonly ISpecializationsOrgUnitReadOnlyRepository _specializationsOrgUnitRepository;
    private readonly ISpecialitySpecializationReadOnlyRepository _specialitySpecializationRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SetStagesPeriodsCommandHandler(
        IStageRepository stageRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        IStudentReadOnlyRepository studentRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        ISpecializationsOrgUnitReadOnlyRepository specializationsOrgUnitRepository,
        ISpecialitySpecializationReadOnlyRepository specialitySpecializationRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _stageRepository = stageRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _studentRepository = studentRepository;
        _employeeRepository = employeeRepository;
        _specializationsOrgUnitRepository = specializationsOrgUnitRepository;
        _specialitySpecializationRepository = specialitySpecializationRepository;
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
        var existingStages = await _stageRepository.GetByOrgUnitAsync(
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
        var overriddenSpecialityIds = existingStages
            .Where(s => s.SpecialityId.HasValue && s.IsActive && !s.IsDeleted)
            .Select(s => s.SpecialityId!.Value)
            .Distinct()
            .ToList();

        // For Supervisors (Teachers)
        var supervisors = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit", 
            orgUnitId, 
            Domain.CommonDomain.Enums.StaffRoleType.Supervisor, 
            cancellationToken);

        var supervisorUserIds = supervisors
            .Where(s => s.IsActive && !s.IsDeleted)
            .Select(s =>
            {
                if (string.IsNullOrEmpty(s.MetadataJson))
                {
                    return request.SpecialityId.HasValue ? null : (int?)s.UserId;
                }
                try
                {
                    var meta = System.Text.Json.JsonSerializer.Deserialize<AWM.Service.Application.Features.Workflow.Supervisors.DTOs.SupervisorAssignmentMetadata>(s.MetadataJson);
                    if (meta != null && meta.SemesterId == request.SemesterId)
                    {
                        if (request.SpecialityId.HasValue)
                        {
                            return meta.SpecialityId == request.SpecialityId.Value ? (int?)s.UserId : null;
                        }
                        else
                        {
                            return (!meta.SpecialityId.HasValue || !overriddenSpecialityIds.Contains(meta.SpecialityId.Value)) ? (int?)s.UserId : null;
                        }
                    }
                }
                catch { }
                return null;
            })
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        if (supervisorUserIds.Any())
        {
            await _notificationService.SendToManyAsync(
                supervisorUserIds,
                "Утверждены сроки этапов воркфлоу",
                currentUserId,
                "Кафедра утвердила сроки этапов формирования направлений, тем и выбора тем. Пожалуйста, ознакомьтесь с ними в системе.",
                null,
                "OrgUnit",
                orgUnitId,
                cancellationToken);
        }

        // For Students
        var studentUserIds = new List<int>();
        if (request.SpecialityId.HasValue)
        {
            var students = await _studentRepository.GetBySpecialityAsync(request.SpecialityId.Value, cancellationToken);
            studentUserIds.AddRange(students.Select(s => s.Id));
        }
        else
        {
            // Fetch all specialities for this OrgUnit (department)
            var specializationsOrgUnits = await _specializationsOrgUnitRepository.GetByOrgUnitAsync(orgUnitId, cancellationToken);
            var specIds = specializationsOrgUnits
                .Where(sou => sou.SpecializationId.HasValue)
                .Select(sou => sou.SpecializationId!.Value)
                .Distinct()
                .ToList();

            var specialityIds = new List<int>();
            foreach (var specId in specIds)
            {
                var specialitySpecs = await _specialitySpecializationRepository.GetBySpecializationAsync(specId, cancellationToken);
                specialityIds.AddRange(specialitySpecs
                    .Where(ss => ss.SpecialityId.HasValue)
                    .Select(ss => ss.SpecialityId!.Value));
            }
            specialityIds = specialityIds.Distinct().ToList();

            // Exclude specialities that have their own Stage override in this semester
            var targetSpecialityIds = specialityIds
                .Where(id => !overriddenSpecialityIds.Contains(id))
                .ToList();

            foreach (var specialityId in targetSpecialityIds)
            {
                var students = await _studentRepository.GetBySpecialityAsync(specialityId, cancellationToken);
                studentUserIds.AddRange(students.Select(s => s.Id));
            }
            studentUserIds = studentUserIds.Distinct().ToList();
        }

        if (studentUserIds.Any())
        {
            await _notificationService.SendToManyAsync(
                studentUserIds,
                "Утверждены сроки этапа выбора тем",
                currentUserId,
                "Утверждены сроки этапа выбора тем. Вы сможете выбрать тему в установленный период.",
                null,
                "OrgUnit",
                orgUnitId,
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}

