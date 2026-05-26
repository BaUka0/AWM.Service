using System.Text.Json;
using System.Linq;
using AWM.Service.Application.Features.Workflow.Supervisors.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Supervisors.Queries.GetSupervisorsStatus;

public sealed class GetSupervisorsStatusQueryHandler : IRequestHandler<GetSupervisorsStatusQuery, Result<SupervisorsStatusDto>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;

    public GetSupervisorsStatusQueryHandler(IStaffAssignmentRepository staffAssignmentRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
    }

    public async Task<Result<SupervisorsStatusDto>> Handle(GetSupervisorsStatusQuery request, CancellationToken cancellationToken)
    {
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
            return Result.Success(new SupervisorsStatusDto(false));
        }

        // Check if any assignment is confirmed
        var isConfirmed = filteredAssignments.Any(a =>
        {
            try
            {
                var meta = JsonSerializer.Deserialize<SupervisorAssignmentMetadata>(a.MetadataJson!);
                return meta?.IsConfirmed == true;
            }
            catch
            {
                return false;
            }
        });

        return Result.Success(new SupervisorsStatusDto(isConfirmed));
    }
}
