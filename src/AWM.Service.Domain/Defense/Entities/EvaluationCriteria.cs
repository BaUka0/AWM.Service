namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// EvaluationCriteria entity - grading criteria for defense evaluation.
/// </summary>
public class EvaluationCriteria : Entity<int>, IAuditable, ISoftDeletable
{
    public int WorkTypeId { get; private set; }
    public int? DepartmentId { get; private set; }
    public string CriteriaName { get; private set; } = null!;
    public int MaxScore { get; private set; }
    public decimal Weight { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private EvaluationCriteria() { }

    public EvaluationCriteria(
        int workTypeId,
        string criteriaName,
        int maxScore,
        int createdBy,
        decimal weight = 1.0m,
        int? departmentId = null)
    {
        if (string.IsNullOrWhiteSpace(criteriaName))
            throw new ArgumentException("Criteria name is required.", nameof(criteriaName));
        if (maxScore <= 0)
            throw new ArgumentException("Max score must be positive.", nameof(maxScore));
        if (weight <= 0)
            throw new ArgumentException("Weight must be positive.", nameof(weight));

        WorkTypeId = workTypeId;
        DepartmentId = departmentId;
        CriteriaName = criteriaName;
        MaxScore = maxScore;
        Weight = weight;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Updates the criteria.
    /// </summary>
    public void Update(string criteriaName, int maxScore, decimal weight, int modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(criteriaName))
            throw new ArgumentException("Criteria name is required.", nameof(criteriaName));
        if (maxScore <= 0)
            throw new ArgumentException("Max score must be positive.", nameof(maxScore));
        if (weight <= 0)
            throw new ArgumentException("Weight must be positive.", nameof(weight));

        CriteriaName = criteriaName;
        MaxScore = maxScore;
        Weight = weight;

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the criteria.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Checks if this is a university-wide criteria (no department).
    /// </summary>
    public bool IsUniversityWide => DepartmentId == null;
}
