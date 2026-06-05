namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.CommonDomain.Enums;
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
            .Include(t => t.Applications)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .Include(t => t.Applications)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.Speciality)
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
    public async Task<IReadOnlyList<Topic>> GetByOrgUnitAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Topics
            .Include(t => t.Applications)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .Where(t => t.OrgUnitId == orgUnitId &&
                        t.SemesterId == semesterId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetByOrgUnitWithApplicationsAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Topics
            .Include(t => t.Applications)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .Where(t => t.OrgUnitId == orgUnitId &&
                        t.SemesterId == semesterId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetBySupervisorAsync(
        int userId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        // Get IDs of topics where the user is assigned as supervisor via StaffAssignments
        var assignedTopicIds = await Context.StaffAssignments
            .AsNoTracking()
            .Where(a => a.UserId == userId &&
                        a.RoleType == StaffRoleType.Supervisor &&
                        a.TargetEntityType == "Topic" &&
                        a.IsActive && !a.IsDeleted)
            .Select(a => a.TargetEntityId)
            .ToListAsync(cancellationToken);

        // Return topics that are either assigned to the user OR created by the user
        return await Context.Topics
            .Include(t => t.Applications)
                .ThenInclude(a => a.Student)
                    .ThenInclude(s => s.User)
            .Where(t => !t.IsDeleted &&
                        t.SemesterId == semesterId &&
                        (assignedTopicIds.Contains(t.Id) || t.CreatedBy == userId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Topic>> GetAvailableForSelectionAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        var acceptedCountsQuery = Context.TopicApplications
            .AsNoTracking()
            .Where(a => a.StatusId == 2)
            .GroupBy(a => a.TopicId)
            .Select(group => new
            {
                TopicId = group.Key,
                AcceptedCount = group.Count()
            });

        return await Context.Topics
            .AsNoTracking()
            .Include(t => t.Applications)
            .Where(t => t.OrgUnitId == orgUnitId &&
                        t.SemesterId == semesterId &&
                        t.Status == TopicStatus.Approved)
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
    public async Task<IReadOnlyList<Topic>> GetByOrgUnitForReconciliationAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        var reconciliationStatuses = new[]
        {
            TopicStatus.Approved,
            TopicStatus.Closed,
            TopicStatus.Reconciled,
            TopicStatus.Inactive,
            TopicStatus.NeedsRevision
        };

        return await Context.Topics
            .Include(t => t.Applications)
            .Where(t => t.OrgUnitId == orgUnitId &&
                        t.SemesterId == semesterId &&
                        reconciliationStatuses.Contains(t.Status))
            .OrderByDescending(t => t.CreatedAt)
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

