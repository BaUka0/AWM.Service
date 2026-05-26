using AWM.Service.Application.Features.Workflow.Supervisors.DTOs;
using System.Text.Json;
using System.Linq;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Commands.UnlockSupervisors;

public sealed class UnlockSupervisorsCommandHandler : IRequestHandler<UnlockSupervisorsCommand, Result>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UnlockSupervisorsCommandHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(UnlockSupervisorsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error(ErrorCodes.Unauthorized, "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var existingAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.OrgUnitId,
            StaffRoleType.Supervisor,
            cancellationToken);

        var filteredAssignments = existingAssignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .Where(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                try
                {
                    var meta = JsonSerializer.Deserialize<SupervisorAssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == request.SemesterId && meta?.SpecialityId == request.SpecialityId;
                }
                catch { return false; }
            })
            .ToList();

        if (filteredAssignments.Count == 0)
        {
            return Result.Success(); // Nothing to unlock
        }

        foreach (var assignment in filteredAssignments)
        {
            SupervisorAssignmentMetadata metadata;
            try
            {
                metadata = JsonSerializer.Deserialize<SupervisorAssignmentMetadata>(assignment.MetadataJson!) 
                           ?? new SupervisorAssignmentMetadata();
            }
            catch
            {
                metadata = new SupervisorAssignmentMetadata();
            }

            if (metadata.IsConfirmed)
            {
                metadata.IsConfirmed = false;
                var metadataJson = JsonSerializer.Serialize(metadata);
                assignment.UpdateMetadata(metadataJson, currentUserId);
                await _staffAssignmentRepository.UpdateAsync(assignment, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
