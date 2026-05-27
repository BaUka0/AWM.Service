using AWM.Service.Application.Features.Workflow.Checks.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Queries.GetActiveCheckConfigurations;

/// <summary>
/// Returns active check configurations for a student's org unit and speciality.
/// Accessible to students (THESIS.CHECK Read).
/// Speciality-specific rules take precedence over global (SpecialityId = null) rules for the same check type.
/// </summary>
public record GetActiveCheckConfigurationsQuery(int OrgUnitId, int? SpecialityId)
    : IRequest<Result<IReadOnlyList<CheckConfigurationDto>>>;

public sealed class GetActiveCheckConfigurationsQueryHandler
    : IRequestHandler<GetActiveCheckConfigurationsQuery, Result<IReadOnlyList<CheckConfigurationDto>>>
{
    private readonly ISpecialityCheckTypeRepository _repo;

    public GetActiveCheckConfigurationsQueryHandler(ISpecialityCheckTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<IReadOnlyList<CheckConfigurationDto>>> Handle(
        GetActiveCheckConfigurationsQuery request, CancellationToken ct)
    {
        var all = await _repo.GetByOrgUnitAsync(request.OrgUnitId, ct);

        var active = all.Where(c => c.IsActive).ToList();

        if (request.SpecialityId.HasValue)
        {
            // Speciality-specific rules override global rules for the same CheckTypeId
            var specificCheckTypeIds = active
                .Where(c => c.SpecialityId == request.SpecialityId)
                .Select(c => c.CheckTypeId)
                .ToHashSet();

            active = active
                .Where(c => c.SpecialityId == request.SpecialityId
                         || (c.SpecialityId == null && !specificCheckTypeIds.Contains(c.CheckTypeId)))
                .ToList();
        }
        else
        {
            // No speciality context — return only global (department-wide) rules
            active = active.Where(c => c.SpecialityId == null).ToList();
        }

        var dtos = active.Select(c => new CheckConfigurationDto(
            c.Id,
            c.OrgUnitId,
            c.CheckTypeId,
            c.CheckType?.Title ?? string.Empty,
            c.CheckType?.Code,
            c.SpecialityId,
            c.Speciality?.Title,
            c.MinimumPassValue,
            c.IsActive
        )).ToList();

        return Result.Success<IReadOnlyList<CheckConfigurationDto>>(dtos);
    }
}
