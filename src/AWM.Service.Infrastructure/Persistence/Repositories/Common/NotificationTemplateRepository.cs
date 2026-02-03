namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for NotificationTemplate.
/// </summary>
public sealed class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationTemplateRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<NotificationTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .Where(t => !t.IsDeleted)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<NotificationTemplate?> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            return null;

        return await _context.NotificationTemplates
            .Where(t => !t.IsDeleted && t.EventType == eventType)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.NotificationTemplates
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.EventType)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        await _context.NotificationTemplates.AddAsync(template, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(NotificationTemplate template, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        _context.NotificationTemplates.Update(template);
        return Task.CompletedTask;
    }
}
