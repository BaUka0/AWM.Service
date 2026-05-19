namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class SemesterTypeRepository : ISemesterTypeRepository
{
    private readonly UniversityDbContext _context;

    public SemesterTypeRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<SemesterType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SemesterTypes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<SemesterType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SemesterTypes.ToListAsync(cancellationToken);
    }
}
