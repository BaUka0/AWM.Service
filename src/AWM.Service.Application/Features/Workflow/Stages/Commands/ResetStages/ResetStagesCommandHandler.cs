using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.ResetStages;

/// <summary>
/// Command handler for ResetStagesCommand.
/// </summary>
public sealed class ResetStagesCommandHandler : IRequestHandler<ResetStagesCommand, Result<Unit>>
{
    private readonly IStageRepository _stageRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ResetStagesCommandHandler(
        IStageRepository stageRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _stageRepository = stageRepository;
        _employeeRepository = employeeRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(ResetStagesCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        // 1. Resolve OrgUnitId
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

        // 2. Fetch all tracked stages for the department and semester
        var existingStages = await _stageRepository.GetTrackedByOrgUnitAsync(
            orgUnitId,
            request.SemesterId,
            cancellationToken);

        // 3. Find specific speciality stages
        var specialityStages = existingStages
            .Where(s => s.SpecialityId == request.SpecialityId && !s.IsDeleted)
            .ToList();

        if (!specialityStages.Any())
        {
            return Result.Success(Unit.Value); // Nothing to reset
        }

        // 4. Soft-delete the overridden stages
        foreach (var stage in specialityStages)
        {
            stage.Delete(currentUserId);
            await _stageRepository.UpdateAsync(stage, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
