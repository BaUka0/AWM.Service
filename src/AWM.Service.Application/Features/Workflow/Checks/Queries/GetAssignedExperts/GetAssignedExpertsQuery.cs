using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Queries.GetAssignedExperts;

public record GetAssignedExpertsQuery(int OrgUnitId) : IRequest<Result<IReadOnlyList<ExpertAssignmentDto>>>;

public sealed class GetAssignedExpertsQueryHandler : IRequestHandler<GetAssignedExpertsQuery, Result<IReadOnlyList<ExpertAssignmentDto>>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICheckTypeRepository _checkTypeRepository;

    public GetAssignedExpertsQueryHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IEmployeeRepository employeeRepository,
        ICheckTypeRepository checkTypeRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _employeeRepository = employeeRepository;
        _checkTypeRepository = checkTypeRepository;
    }

    public async Task<Result<IReadOnlyList<ExpertAssignmentDto>>> Handle(GetAssignedExpertsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit",
            request.OrgUnitId,
            StaffRoleType.QualityExpert,
            cancellationToken);

        var checkTypes = await _checkTypeRepository.GetAllAsync(cancellationToken);
        var checkTypeMap = checkTypes.ToDictionary(c => c.Id, c => c.Title);

        var activeAssignments = assignments.Where(a => !a.IsDeleted).ToList();
        if (!activeAssignments.Any())
        {
            return Result.Success<IReadOnlyList<ExpertAssignmentDto>>(new List<ExpertAssignmentDto>());
        }

        var employees = await _employeeRepository.GetByOrgUnitAsync(request.OrgUnitId, cancellationToken);
        var employeeMap = employees
            .Where(e => e.User != null)
            .ToDictionary(e => e.User!.Id, e => $"{e.User!.LastName} {e.User!.FirstName} {e.User!.MiddleName}".Trim());

        var dtos = new List<ExpertAssignmentDto>();
        foreach (var a in activeAssignments)
        {
            if (string.IsNullOrEmpty(a.MetadataJson)) continue;

            int checkTypeId = 0;
            try
            {
                using var doc = JsonDocument.Parse(a.MetadataJson);
                if (doc.RootElement.TryGetProperty("CheckTypeId", out var prop) && prop.ValueKind == JsonValueKind.Number)
                {
                    checkTypeId = prop.GetInt32();
                }
            }
            catch { }

            if (checkTypeId == 0) continue;

            var fullName = employeeMap.TryGetValue(a.UserId, out var name) ? name : $"Преподаватель #{a.UserId}";
            var checkTypeName = checkTypeMap.TryGetValue(checkTypeId, out var cName) ? cName : $"Проверка #{checkTypeId}";

            dtos.Add(new ExpertAssignmentDto(
                a.Id,
                a.UserId,
                fullName,
                checkTypeId,
                checkTypeName,
                a.IsActive));
        }

        return Result.Success<IReadOnlyList<ExpertAssignmentDto>>(dtos);
    }
}
