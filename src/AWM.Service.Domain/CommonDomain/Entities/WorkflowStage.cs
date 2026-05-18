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
            throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
        OrderBy = orderBy;
    }
}
