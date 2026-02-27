namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for StudentWork aggregate.
/// </summary>
public sealed class StudentWorkRepository : RepositoryBase<StudentWork, long>, IStudentWorkRepository
{
    public StudentWorkRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public override async Task<StudentWork?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Context.StudentWorks
            .Include(w => w.Participants)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<StudentWork?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Context.StudentWorks
            .Include(w => w.Participants)
            .Include(w => w.Attachments)
            .Include(w => w.QualityChecks)
            .Include(w => w.WorkflowHistory)
            .AsSplitQuery() // Split for performance with multiple collections
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentWork>> GetByStudentAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return await Context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => w.Participants.Any(p => p.StudentId == studentId))
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentWork>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => w.DepartmentId == departmentId &&
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
        return await Context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => w.AcademicYearId == academicYearId &&
                        Context.Topics.Any(t => t.Id == w.TopicId &&
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
        return await Context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => w.CurrentStateId == stateId &&
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
        var query = Context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => w.DepartmentId == departmentId &&
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
        var query = Context.StudentWorks
            .AsNoTracking()
            .Include(w => w.Participants)
            .Where(w => w.CurrentStateId == stateId &&
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
    public override async Task AddAsync(StudentWork work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        await Context.StudentWorks.AddAsync(work, cancellationToken);
    }

    /// <inheritdoc />
    public override Task UpdateAsync(StudentWork work, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(work);
        Context.StudentWorks.Update(work);
        return Task.CompletedTask;
    }
}
