namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Workflow stage reference entity replacing the WorkflowStage enum.
/// Maps to [Common].[WorkflowStages].
/// </summary>
public class WorkflowStage : Entity<int>
{
    public string Name { get; private set; } = null!;
    public int OrderBy { get; private set; }

    private WorkflowStage() { }

    public WorkflowStage(string name, int orderBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("WorkflowStage.NameRequired", "Name is required.");

        Name = name;
        OrderBy = orderBy;
    }

    public WorkflowStage(int id, string name, int orderBy) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("WorkflowStage.NameRequired", "Name is required.");

        Name = name;
        OrderBy = orderBy;
    }
}
