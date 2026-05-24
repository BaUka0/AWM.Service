namespace AWM.Service.Infrastructure.Persistence.Repositories.University;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class EmployeeReadOnlyRepository : IEmployeeReadOnlyRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public EmployeeReadOnlyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .Where(e => ids.Contains(e.Id))
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAdvisorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Where(e => e.IsAdvisor)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }
}
