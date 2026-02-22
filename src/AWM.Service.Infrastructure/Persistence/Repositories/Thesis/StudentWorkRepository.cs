namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for StudentWork aggregate.
/// </summary>
public sealed class StudentWorkRepository : IStudentWorkRepository
{
    private readonly ApplicationDbContext _context;

    public StudentWorkRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<StudentWork?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.StudentWorks
            .Include(w => w.Participants)
            .Where(w => !w.IsDeleted)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<StudentWork?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.StudentWorks
            .Include(w => w.Participants)
            .Include(w => w.Attachments)
            .Include(w => w.QualityChecks)
            .Include(w => w.WorkflowHistory)
            .Where(w => !w.IsDeleted)
            .AsSplitQuery() // Split for performance with multiple collections
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentWork>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => !w.IsDeleted &&
                        w.Participants.Any(p => p.StudentId == studentId))
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentWork>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => !w.IsDeleted &&
                        w.DepartmentId == departmentId &&
                        w.AcademicYearId == academicYearId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentWork>> GetBySupervisorAsync(
        int supervisorId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => !w.IsDeleted &&
                        w.AcademicYearId == academicYearId &&
                        _context.Topics.Any(t => t.Id == w.TopicId &&
                                                 t.SupervisorId == supervisorId))
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentWork>> GetByStateAsync(
        int stateId,
        int departmentId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => !w.IsDeleted &&
                        w.CurrentStateId == stateId &&
                        w.DepartmentId == departmentId)
            .OrderByDescending(w => w.LastModifiedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<StudentWork> Items, int TotalCount)> GetByDepartmentPagedAsync(
        int departmentId,
        int academicYearId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => !w.IsDeleted &&
                        w.DepartmentId == departmentId &&
                        w.AcademicYearId == academicYearId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<StudentWork> Items, int TotalCount)> GetByStatePagedAsync(
        int stateId,
        int departmentId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => !w.IsDeleted &&
                        w.CurrentStateId == stateId &&
                        w.DepartmentId == departmentId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(w => w.LastModifiedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task AddAsync(StudentWork work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        await _context.StudentWorks.AddAsync(work, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(StudentWork work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        _context.StudentWorks.Update(work);
        return Task.CompletedTask;
    }
}

