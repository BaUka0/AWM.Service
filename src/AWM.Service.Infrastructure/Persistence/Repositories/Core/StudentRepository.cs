namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly UniversityDbContext _context;

    public StudentRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Students.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetBySpecialityAsync(int specialityId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => s.SpecialityId == specialityId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByStatusAsync(int statusId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => s.StatusId == statusId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Students.ToListAsync(cancellationToken);
    }
}
