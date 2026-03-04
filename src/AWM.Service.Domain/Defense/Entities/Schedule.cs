namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Schedule entity - defense time slot for a student work.
/// </summary>
public class Schedule : Entity<long>, IAuditable, ISoftDeletable
{
    public int CommissionId { get; private set; }
    public long WorkId { get; private set; }
    public DateTime DefenseDate { get; private set; }
    public string? Location { get; private set; }
    public bool IsReconciliationStarted { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private readonly List<Grade> _grades = new();
    public IReadOnlyCollection<Grade> Grades => _grades.AsReadOnly();

    private Schedule() { }

    public Schedule(int commissionId, long workId, DateTime defenseDate, int createdBy, string? location = null)
    {
        if (defenseDate < DateTime.UtcNow)
            throw new ArgumentException("Defense date cannot be in the past.", nameof(defenseDate));

        CommissionId = commissionId;
        WorkId = workId;
        DefenseDate = defenseDate;
        Location = location;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
        IsReconciliationStarted = false;
    }

    /// <summary>
    /// Reschedules the defense.
    /// </summary>
    public void Reschedule(DateTime newDate, int modifiedBy, string? newLocation = null)
    {
        DefenseDate = newDate;
        Location = newLocation;

        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Assigns a student work to this schedule slot.
    /// </summary>
    public void AssignWork(long workId, int modifiedBy)
    {
        WorkId = workId;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Updates the location.
    /// </summary>
    public void UpdateLocation(string location, int modifiedBy)
    {
        Location = location;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Adds a grade from a commission member.
    /// </summary>
    public Grade AddGrade(int memberId, int criteriaId, int score, string? comment = null)
    {
        // Check if this member already graded this criteria
        if (_grades.Any(g => g.MemberId == memberId && g.CriteriaId == criteriaId))
            throw new InvalidOperationException("Member has already graded this criteria.");

        var grade = new Grade(Id, memberId, criteriaId, score, comment);
        _grades.Add(grade);

        LastModifiedAt = DateTime.UtcNow;
        return grade;
    }

    /// <summary>
    /// Starts the grade reconciliation phase (all members can see each other's grades).
    /// </summary>
    public void StartReconciliation(int modifiedBy)
    {
        if (IsReconciliationStarted)
            throw new InvalidOperationException("Reconciliation has already been started.");

        IsReconciliationStarted = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the schedule.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Calculates average score from all grades.
    /// </summary>
    public decimal? GetAverageScore()
    {
        if (!_grades.Any())
            return null;

        return (decimal)_grades.Average(g => g.Score);
    }
}
