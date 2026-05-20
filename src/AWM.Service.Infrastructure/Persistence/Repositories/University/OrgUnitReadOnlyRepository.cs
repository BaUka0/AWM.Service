namespace AWM.Service.Infrastructure.Persistence.Repositories.University;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class OrgUnitReadOnlyRepository : IOrgUnitReadOnlyRepository
{
    private readonly UniversityDbContext _context;

    // TypeId constants from Edu_OrgUnitTypes
    private const int TypeDepartment = 1;
    private const int TypeInstitute = 2;

    public OrgUnitReadOnlyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<OrgUnit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => ids.Contains(o.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetByTypeAsync(int typeId, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => o.TypeId == typeId && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => o.TypeId == TypeDepartment && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetInstitutesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => o.TypeId == TypeInstitute && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetChildrenAsync(int parentId, CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => o.ParentId == parentId && !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnits
            .Where(o => !o.Deleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrgUnitType>> GetAllTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrgUnitTypes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
