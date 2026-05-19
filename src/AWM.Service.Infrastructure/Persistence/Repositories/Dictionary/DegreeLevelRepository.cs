namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class DegreeLevelRepository : IDegreeLevelRepository
{
    private readonly UniversityDbContext _context;

    public DegreeLevelRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<SpecialityLevel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SpecialityLevels.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialityLevel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SpecialityLevels.ToListAsync(cancellationToken);
    }
}
