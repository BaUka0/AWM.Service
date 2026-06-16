using AWM.Service.Application.Features.Workflow.Stages.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Stages.Queries.GetOrgUnitSpecialities;

/// <summary>
/// Handler for GetOrgUnitSpecialitiesQuery.
/// </summary>
public sealed class GetOrgUnitSpecialitiesQueryHandler : IRequestHandler<GetOrgUnitSpecialitiesQuery, Result<IReadOnlyList<SpecialityDto>>>
{
    private readonly ISpecializationsOrgUnitReadOnlyRepository _specializationsOrgUnitRepository;
    private readonly ISpecialitySpecializationReadOnlyRepository _specialitySpecializationRepository;
    private readonly ISpecialityReadOnlyRepository _specialityRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetOrgUnitSpecialitiesQueryHandler(
        ISpecializationsOrgUnitReadOnlyRepository specializationsOrgUnitRepository,
        ISpecialitySpecializationReadOnlyRepository specialitySpecializationRepository,
        ISpecialityReadOnlyRepository specialityRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _specializationsOrgUnitRepository = specializationsOrgUnitRepository;
        _specialitySpecializationRepository = specialitySpecializationRepository;
        _specialityRepository = specialityRepository;
        _employeeRepository = employeeRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<SpecialityDto>>> Handle(GetOrgUnitSpecialitiesQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<SpecialityDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

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
                return Result.Failure<IReadOnlyList<SpecialityDto>>(new Error("Stages.EmployeeNotFound", "Employee record not found for the current user in University SoT."));
            }

            var mainPosition = employee.Positions.FirstOrDefault(p => p.IsMainPosition)
                               ?? employee.Positions.FirstOrDefault();

            if (mainPosition == null)
            {
                return Result.Failure<IReadOnlyList<SpecialityDto>>(new Error("Stages.OrgUnitNotFound", "Employee has no assigned department in University SoT."));
            }

            orgUnitId = mainPosition.OrgUnitId;
        }

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

        var dtos = new List<SpecialityDto>();
        foreach (var specialityId in specialityIds)
        {
            var speciality = await _specialityRepository.GetByIdAsync(specialityId, cancellationToken);
            if (speciality != null && !speciality.Deleted)
            {
                dtos.Add(new SpecialityDto(speciality.Id, speciality.Code, speciality.Title));
            }
        }

        return Result.Success<IReadOnlyList<SpecialityDto>>(dtos.OrderBy(s => s.Code).ToList());
    }
}
