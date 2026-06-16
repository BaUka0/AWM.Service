namespace AWM.Service.Infrastructure.Persistence.Repositories.University;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class SpecializationsOrgUnitReadOnlyRepository : ISpecializationsOrgUnitReadOnlyRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public SpecializationsOrgUnitReadOnlyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SpecializationsOrgUnit>> GetBySpecializationAsync(int specializationId, CancellationToken cancellationToken = default)
    {
        return await _context.SpecializationsOrgUnits
            .Where(sou => sou.SpecializationId == specializationId)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SpecializationsOrgUnit>> GetByOrgUnitAsync(int orgUnitId, CancellationToken cancellationToken = default)
    {
        return await _context.SpecializationsOrgUnits
            .Where(sou => sou.OrgUnitId == orgUnitId)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SpecializationsOrgUnit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SpecializationsOrgUnits
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }
}
