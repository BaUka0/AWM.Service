namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;
using AWM.Service.Domain.Repositories;

public class SemesterRepository : ISemesterRepository
{
    private readonly UniversityDbContext _context;

    public SemesterRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Semester?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Semesters.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Semester?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Semesters
            .Where(s => s.StartsOn <= now && s.EndsOn >= now)
            .OrderByDescending(s => s.StartsOn)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Semester>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Semesters
            .OrderByDescending(s => s.StudyYear)
            .ThenBy(s => s.SemesterTypeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Semester>> GetByStudyYearAsync(int studyYear, CancellationToken cancellationToken = default)
    {
        return await _context.Semesters
            .Where(s => s.StudyYear == studyYear)
            .OrderBy(s => s.StartsOn)
            .ToListAsync(cancellationToken);
    }
}
