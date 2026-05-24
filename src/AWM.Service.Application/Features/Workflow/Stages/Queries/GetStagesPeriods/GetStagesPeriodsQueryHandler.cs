using AWM.Service.Application.Features.Workflow.Stages.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Stages.Queries.GetStagesPeriods;

public sealed class GetStagesPeriodsQueryHandler : IRequestHandler<GetStagesPeriodsQuery, Result<IReadOnlyList<StagePeriodDto>>>
{
    private readonly IStageRepository _stageRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetStagesPeriodsQueryHandler(
        IStageRepository stageRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _stageRepository = stageRepository;
        _employeeRepository = employeeRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<StagePeriodDto>>> Handle(GetStagesPeriodsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<StagePeriodDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
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
                return Result.Failure<IReadOnlyList<StagePeriodDto>>(new Error("Stages.EmployeeNotFound", "Employee record not found for the current user."));
            }

            var mainPosition = employee.Positions.FirstOrDefault(p => p.IsMainPosition) 
                               ?? employee.Positions.FirstOrDefault();
            
            if (mainPosition == null)
            {
                return Result.Failure<IReadOnlyList<StagePeriodDto>>(new Error("Stages.OrgUnitNotFound", "Employee has no assigned department."));
            }

            orgUnitId = mainPosition.OrgUnitId;
        }

        // 2. Fetch all stages for the department and semester
        var allStages = await _stageRepository.GetByDepartmentAsync(orgUnitId, request.SemesterId, cancellationToken);

        // 3. Filter stages with fallback logic
        var activeStages = allStages.Where(s => s.IsActive && !s.IsDeleted).ToList();

        List<Domain.CommonDomain.Entities.Stage> filteredStages;

        if (request.SpecialityId.HasValue)
        {
            // Try specific speciality stages
            filteredStages = activeStages.Where(s => s.SpecialityId == request.SpecialityId.Value).ToList();

            // Fallback to department-wide stages if no speciality-specific stages exist
            if (!filteredStages.Any())
            {
                filteredStages = activeStages.Where(s => s.SpecialityId == null).ToList();
            }
        }
        else
        {
            // Department-wide stages only
            filteredStages = activeStages.Where(s => s.SpecialityId == null).ToList();
        }

        var dtos = filteredStages
            .Select(s => new StagePeriodDto(s.WorkflowStageId, s.StartDate, s.EndDate))
            .ToList();

        return Result.Success<IReadOnlyList<StagePeriodDto>>(dtos);
    }
}
