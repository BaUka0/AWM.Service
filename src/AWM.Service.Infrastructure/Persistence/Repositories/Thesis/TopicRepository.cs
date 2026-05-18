namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Topic aggregate.
/// </summary>
public sealed class TopicRepository : RepositoryBase<Topic, long>, ITopicRepository
{
    public TopicRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public override async Task<Topic?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await Context.Topics
            .Include(t => t.Applications.Where(a => !a.IsDeleted))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await Context.Topics
            .AsNoTracking()
            .Where(t => idList.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Topics
            .AsNoTracking()
            .Where(t => t.DepartmentId == departmentId &&
                        t.AcademicYearId == academicYearId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetByDepartmentWithApplicationsAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Topics
            .Include(t => t.Applications.Where(a => !a.IsDeleted))
            .Where(t => t.DepartmentId == departmentId &&
                        t.AcademicYearId == academicYearId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetBySupervisorAsync(
        int supervisorId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Topics
            .AsNoTracking()
            .Where(t => t.SupervisorId == supervisorId &&
                        t.AcademicYearId == academicYearId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetAvailableForSelectionAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var acceptedCountsQuery = Context.TopicApplications
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.StatusId == 2)
            .GroupBy(a => a.TopicId)
            .Select(group => new
            {
                TopicId = group.Key,
                AcceptedCount = group.Count()
            });

        return await Context.Topics
            .AsNoTracking()
            .Where(t => !t.IsDeleted &&
                        t.DepartmentId == departmentId &&
                        t.AcademicYearId == academicYearId &&
                        t.IsApproved &&
                        !t.IsClosed)
            .GroupJoin(
                acceptedCountsQuery,
                topic => topic.Id,
                counter => counter.TopicId,
                (topic, counters) => new
                {
                    Topic = topic,
                    AcceptedCount = counters.Select(counter => (int?)counter.AcceptedCount).FirstOrDefault() ?? 0
                })
            .Where(item => item.AcceptedCount < item.Topic.MaxParticipants)
            .OrderByDescending(item => item.Topic.MaxParticipants - item.AcceptedCount)
            .ThenByDescending(item => item.Topic.CreatedAt)
            .Select(item => item.Topic)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TopicApplication>> GetApplicationsByStudentIdAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        return await Context.TopicApplications
            .AsNoTracking()
            .Where(a => a.StudentId == studentId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override async Task AddAsync(Topic topic, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(topic);
        await Context.Topics.AddAsync(topic, cancellationToken);
    }

    /// <inheritdoc />
    public override Task UpdateAsync(Topic topic, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(topic);
        Context.Topics.Update(topic);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Topic topic, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(topic);
        Context.Topics.Update(topic);
        return Task.CompletedTask;
    }
}
