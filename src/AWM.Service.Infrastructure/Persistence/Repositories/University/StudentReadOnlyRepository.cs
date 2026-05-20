namespace AWM.Service.Infrastructure.Persistence.Repositories.University;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class StudentReadOnlyRepository : IStudentReadOnlyRepository
{
    private readonly UniversityDbContext _context;

    private const int MaxQuerySize = 1000;

    public StudentReadOnlyRepository(UniversityDbContext context)
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
            .FirstOrDefaultAsync(s => s.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetBySpecialityAsync(int specialityId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => s.SpecialityId == specialityId)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByStatusAsync(int statusId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => s.StatusId == statusId)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetByYearAsync(int year, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => s.Year == year)
            .Take(MaxQuerySize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Students.Take(MaxQuerySize).ToListAsync(cancellationToken);
    }
}
