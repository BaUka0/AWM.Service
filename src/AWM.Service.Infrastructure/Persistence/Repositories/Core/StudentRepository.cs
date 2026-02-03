namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Student aggregate.
/// </summary>
public sealed class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Student>> GetByProgramAsync(int programId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.ProgramId == programId)
            .OrderBy(s => s.CurrentCourse)
            .ThenBy(s => s.GroupCode)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(student);
        await _context.Students.AddAsync(student, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(student);
        _context.Students.Update(student);
        return Task.CompletedTask;
    }
}
