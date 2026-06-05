using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using System.Text.Json;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Employees.Commands.UpdateEmployeeWorkload;

public sealed class UpdateEmployeeWorkloadCommandHandler : IRequestHandler<UpdateEmployeeWorkloadCommand, Result<Unit>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateEmployeeWorkloadCommandHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(UpdateEmployeeWorkloadCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.Unauthorized, "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var assignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.OrgUnitId,
            StaffRoleType.Supervisor,
            cancellationToken);

        var assignment = assignments
            .Where(a => a.IsActive && !a.IsDeleted && a.UserId == request.UserId)
            .FirstOrDefault(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                try
                {
                    var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == request.SemesterId && meta?.SpecialityId == request.SpecialityId;
                }
                catch { return false; }
            });

        if (assignment == null)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.NotFound, "Employee assignment not found."));
        }

        var metadata = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(assignment.MetadataJson!);
        metadata!.MaxWorkload = request.MaxWorkload;

        assignment.UpdateMetadata(JsonSerializer.Serialize(metadata), currentUserId);
        await _staffAssignmentRepository.UpdateAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
