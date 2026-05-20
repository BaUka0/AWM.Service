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
            .Where(s => specialityIds.Contains(s.SpecialityId))
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
