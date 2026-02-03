namespace AWM.Service.Domain.Edu.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Degree level entity (Bachelor, Master, PhD).
/// </summary>
public class DegreeLevel : Entity<int>, IAuditable
{
    public string Name { get; private set; } = null!;
    public int DurationYears { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private DegreeLevel() { }

    public DegreeLevel(string name, int durationYears, int createdBy = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Degree level name is required.", nameof(name));
        if (durationYears <= 0)
            throw new ArgumentException("Duration must be positive.", nameof(durationYears));

        Name = name;
        DurationYears = durationYears;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Creates Bachelor degree level.
    /// </summary>
    public static DegreeLevel Bachelor(int createdBy = 0) => new("Bachelor", 4, createdBy);

    /// <summary>
    /// Creates Master degree level.
    /// </summary>
    public static DegreeLevel Master(int createdBy = 0) => new("Master", 2, createdBy);

    /// <summary>
    /// Creates PhD degree level.
    /// </summary>
    public static DegreeLevel PhD(int createdBy = 0) => new("PhD", 3, createdBy);
}
