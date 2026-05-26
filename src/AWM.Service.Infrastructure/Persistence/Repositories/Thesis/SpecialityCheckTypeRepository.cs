namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Repositories;

public class SpecialityCheckTypeRepository : ISpecialityCheckTypeRepository
{
    private readonly ApplicationDbContext _context;

    public SpecialityCheckTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SpecialityCheckType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialityCheckTypes
            .Include(s => s.CheckType)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<SpecialityCheckType?> GetByCompositeKeyAsync(int orgUnitId, int checkTypeId, int? specialityId, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialityCheckTypes
            .Include(s => s.CheckType)
            .FirstOrDefaultAsync(s => s.OrgUnitId == orgUnitId && s.CheckTypeId == checkTypeId && s.SpecialityId == specialityId, cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialityCheckType>> GetByOrgUnitAsync(int orgUnitId, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialityCheckTypes
            .Where(s => s.OrgUnitId == orgUnitId)
            .Include(s => s.CheckType)
            .Include(s => s.Speciality)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialityCheckType>> GetBySpecialityAsync(int specialityId, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialityCheckTypes
            .Where(s => s.SpecialityId == specialityId)
            .Include(s => s.CheckType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialityCheckType>> GetBySpecialitiesAsync(IEnumerable<int> specialityIds, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialityCheckTypes
            .Where(s => s.SpecialityId != null && specialityIds.Contains(s.SpecialityId.Value))
            .Include(s => s.CheckType)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SpecialityCheckType specialityCheckType, CancellationToken cancellationToken = default)
    {
        await _context.SpecialityCheckTypes.AddAsync(specialityCheckType, cancellationToken);
    }

    public async Task DeleteAsync(SpecialityCheckType specialityCheckType, CancellationToken cancellationToken = default)
    {
        _context.SpecialityCheckTypes.Remove(specialityCheckType);
        await Task.CompletedTask;
    }
}
