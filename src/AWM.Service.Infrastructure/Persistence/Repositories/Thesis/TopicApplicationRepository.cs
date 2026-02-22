namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for TopicApplication aggregate.
/// </summary>
public sealed class TopicApplicationRepository : ITopicApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public TopicApplicationRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<TopicApplication?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.TopicApplications
            .Where(a => !a.IsDeleted)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TopicApplication?> GetByIdWithTopicAsync(long id, CancellationToken cancellationToken = default)
    {
        // This method signature suggests we might want to load the Topic along with the Application,
        // but ApplicationDbContext doesn't have a direct navigation property from TopicApplication to Topic 
        // in standard DDD unless configured. We can just load the application. The query handlers load Topic separately.
        return await _context.TopicApplications
            .Where(a => !a.IsDeleted)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TopicApplication>> GetByTopicIdAsync(long topicId, CancellationToken cancellationToken = default)
    {
        return await _context.TopicApplications
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.TopicId == topicId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TopicApplication>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.TopicApplications
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.StudentId == studentId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TopicApplication>> GetByStudentIdAndYearAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default)
    {
        // Since TopicApplication doesn't directly have AcademicYearId, we join with Topics
        return await _context.TopicApplications
            .AsNoTracking()
            .Join(_context.Topics,
                  app => app.TopicId,
                  topic => topic.Id,
                  (app, topic) => new { App = app, Topic = topic })
            .Where(x => !x.App.IsDeleted &&
                        !x.Topic.IsDeleted &&
                        x.App.StudentId == studentId &&
                        x.Topic.AcademicYearId == academicYearId)
            .Select(x => x.App)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasStudentAppliedToTopicAsync(int studentId, long topicId, CancellationToken cancellationToken = default)
    {
        return await _context.TopicApplications
            .AnyAsync(a => !a.IsDeleted && a.StudentId == studentId && a.TopicId == topicId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasAcceptedApplicationAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default)
    {
        return await _context.TopicApplications
            .Join(_context.Topics,
                  app => app.TopicId,
                  topic => topic.Id,
                  (app, topic) => new { App = app, Topic = topic })
            .AnyAsync(x => !x.App.IsDeleted &&
                           !x.Topic.IsDeleted &&
                           x.App.StudentId == studentId &&
                           x.Topic.AcademicYearId == academicYearId &&
                           x.App.Status == Domain.Thesis.Enums.ApplicationStatus.Accepted,
                           cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(TopicApplication application, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);
        await _context.TopicApplications.AddAsync(application, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(TopicApplication application, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);
        _context.TopicApplications.Update(application);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(TopicApplication application, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);
        _context.TopicApplications.Update(application);
        return Task.CompletedTask;
    }
}
