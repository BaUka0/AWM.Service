using System.Text.Json;
using System.Linq;
using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Employees.Queries.GetEmployeesStatus;

public sealed class GetEmployeesStatusQueryHandler : IRequestHandler<GetEmployeesStatusQuery, Result<EmployeesStatusDto>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;

    public GetEmployeesStatusQueryHandler(IStaffAssignmentRepository staffAssignmentRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
    }

    public async Task<Result<EmployeesStatusDto>> Handle(GetEmployeesStatusQuery request, CancellationToken cancellationToken)
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
                    var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == request.SemesterId && meta?.SpecialityId == request.SpecialityId;
                }
                catch { return false; }
            })
            .ToList();

        if (filteredAssignments.Count == 0)
        {
            return Result.Success(new EmployeesStatusDto(false));
        }

        // Check if any assignment is confirmed
        var isConfirmed = filteredAssignments.Any(a =>
        {
            try
            {
                var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson!);
                return meta?.IsConfirmed == true;
            }
            catch
            {
                return false;
            }
        });

        return Result.Success(new EmployeesStatusDto(isConfirmed));
    }
}
