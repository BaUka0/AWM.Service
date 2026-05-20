namespace AWM.Service.Infrastructure.Persistence.Repositories.University;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class SpecialityReadOnlyRepository : ISpecialityReadOnlyRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public SpecialityReadOnlyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Speciality?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Specialities.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<Speciality>> GetByLevelAsync(int levelId, CancellationToken cancellationToken = default)
    {
        return await _context.Specialities
            .Where(s => s.LevelId == levelId && !s.Deleted)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Speciality>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Specialities
            .Where(s => !s.Deleted)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }
}
