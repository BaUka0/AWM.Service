namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class OrganizationLookupRepository : IOrganizationLookupRepository
{
    private readonly UniversityDbContext _context;

    // TypeId constants from Edu_OrgUnitTypes
    private const int TypeDepartment = 1;
    private const int TypeInstitute = 2;

    public OrganizationLookupRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrgUnit>> GetAllInstitutesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => o.TypeId == TypeInstitute && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetDepartmentsByInstituteAsync(int instituteId, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => o.ParentId == instituteId && o.TypeId == TypeDepartment && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => o.TypeId == TypeDepartment && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrgUnit?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .FirstOrDefaultAsync(o => o.Id == id && o.TypeId == TypeDepartment && !o.Deleted, cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetDepartmentsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => ids.Contains(o.Id) && o.TypeId == TypeDepartment && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrgUnit?> GetInstituteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .FirstOrDefaultAsync(o => o.Id == id && o.TypeId == TypeInstitute && !o.Deleted, cancellationToken);
    }

    public async Task<OrgUnit?> GetInstituteByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .AsTracking()
            .FirstOrDefaultAsync(o => o.Id == id && o.TypeId == TypeInstitute && !o.Deleted, cancellationToken);
    }

    public async Task<OrgUnit?> GetDepartmentByIdTrackedAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .AsTracking()
            .FirstOrDefaultAsync(o => o.Id == id && o.TypeId == TypeDepartment && !o.Deleted, cancellationToken);
    }
}
