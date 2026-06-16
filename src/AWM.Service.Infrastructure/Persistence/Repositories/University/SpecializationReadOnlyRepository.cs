namespace AWM.Service.Infrastructure.Persistence.Repositories.University;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class SpecializationReadOnlyRepository : ISpecializationReadOnlyRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public SpecializationReadOnlyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Specialization?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Specializations.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<Specialization>> GetByOrgUnitAsync(int orgUnitId, CancellationToken cancellationToken = default)
    {
        var specializationIds = await _context.SpecializationsOrgUnits
            .Where(sou => sou.OrgUnitId == orgUnitId)
            .Select(sou => sou.SpecializationId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await _context.Specializations
            .Where(s => specializationIds.Contains(s.Id))
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Specialization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Specializations
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }
}
