namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public EmployeeRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Employee?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => ids.Contains(e.Id))
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAdvisorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Where(e => e.IsAdvisor)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .Include(e => e.Positions)
                .ThenInclude(p => p.OrgUnit)
            .Where(e => e.Positions.Any(ep => ep.OrgUnitId == departmentId))
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAllWithPositionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .Include(e => e.Positions)
                .ThenInclude(p => p.OrgUnit)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Employee?> GetByIdWithPositionsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Positions)
                .ThenInclude(p => p.Position)
            .Include(e => e.Positions)
                .ThenInclude(p => p.OrgUnit)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees.Take(MaxQuerySize).ToListAsync(cancellationToken);
    }
}
