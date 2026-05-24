using System.Text.Json;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.RemoveSupervisor;

public sealed class RemoveSupervisorCommandHandler : IRequestHandler<RemoveSupervisorCommand, Result<Unit>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RemoveSupervisorCommandHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<Unit>> Handle(RemoveSupervisorCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.Unauthorized, "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var assignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.DepartmentId,
            StaffRoleType.Supervisor,
            cancellationToken);

        var assignment = assignments
            .Where(a => a.IsActive && !a.IsDeleted && a.UserId == request.UserId)
            .FirstOrDefault(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                try
                {
                    var meta = JsonSerializer.Deserialize<AssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == request.SemesterId && meta?.SpecialityId == request.SpecialityId;
                }
                catch { return false; }
            });

        if (assignment == null)
        {
            return Result.Failure<Unit>(new Error(ErrorCodes.NotFound, "Supervisor assignment not found."));
        }

        assignment.Deactivate(currentUserId);
        await _staffAssignmentRepository.UpdateAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }

    private class AssignmentMetadata
    {
        public int SemesterId { get; set; }
        public int? SpecialityId { get; set; }
    }
}
