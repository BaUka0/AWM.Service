namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Student aggregate.
/// </summary>
public sealed class StudentRepository : RepositoryBase<Student, int>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Students
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Student>> GetByProgramAsync(int programId, CancellationToken cancellationToken = default)
    {
        return await Context.Students
            .AsNoTracking()
            .Where(s => s.ProgramId == programId)
            .OrderBy(s => s.CurrentCourse)
            .ThenBy(s => s.GroupCode)
            .ToListAsync(cancellationToken);
    }
}
