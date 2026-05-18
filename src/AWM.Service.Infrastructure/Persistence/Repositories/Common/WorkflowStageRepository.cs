namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for WorkflowStage.
/// </summary>
public sealed class WorkflowStageRepository : RepositoryBase<WorkflowStage, int>, IWorkflowStageRepository
{
    public WorkflowStageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<WorkflowStage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.WorkflowStages
            .AsNoTracking()
            .OrderBy(w => w.OrderBy)
            .ToListAsync(cancellationToken);
    }
}
