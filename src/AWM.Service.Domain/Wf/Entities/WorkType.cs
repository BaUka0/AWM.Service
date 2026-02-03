namespace AWM.Service.Domain.Wf.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Work type entity - classifies types of academic works.
/// Examples: CourseWork, DiplomaWork, MasterThesis, PhD.
/// </summary>
public class WorkType : Entity<int>, IAuditable, ISoftDeletable
{
    public string Name { get; private set; } = null!;
    public int? DegreeLevelId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<State> _states = new();
    public IReadOnlyCollection<State> States => _states.AsReadOnly();

    private WorkType() { }

    public WorkType(string name, int createdBy = 0, int? degreeLevelId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Work type name is required.", nameof(name));

        Name = name;
        DegreeLevelId = degreeLevelId;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Soft deletes the work type.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Creates CourseWork type.
    /// </summary>
    public static WorkType CourseWork(int createdBy = 0) => new("CourseWork", createdBy);

    /// <summary>
    /// Creates DiplomaWork (Bachelor) type.
    /// </summary>
    public static WorkType DiplomaWork(int bachelorDegreeLevelId, int createdBy = 0) 
        => new("DiplomaWork", createdBy, bachelorDegreeLevelId);

    /// <summary>
    /// Creates MasterThesis type.
    /// </summary>
    public static WorkType MasterThesis(int masterDegreeLevelId, int createdBy = 0) 
        => new("MasterThesis", createdBy, masterDegreeLevelId);

    /// <summary>
    /// Creates PhD thesis type.
    /// </summary>
    public static WorkType PhD(int phdDegreeLevelId, int createdBy = 0) 
        => new("PhD", createdBy, phdDegreeLevelId);
}
