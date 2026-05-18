namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Semester.
/// </summary>
public sealed class SemesterRepository : RepositoryBase<Semester, int>, ISemesterRepository
{
    public SemesterRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Semester>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Semesters
            .AsNoTracking()
            .OrderByDescending(s => s.StudyYear)
            .ThenBy(s => s.StartsOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Semester>> GetByStudyYearAsync(int studyYear, CancellationToken cancellationToken = default)
    {
        return await Context.Semesters
            .AsNoTracking()
            .Where(s => s.StudyYear == studyYear)
            .OrderBy(s => s.StartsOn)
            .ToListAsync(cancellationToken);
    }
}
