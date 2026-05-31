namespace AWM.Service.Domain.Wf.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// State entity - represents a state in the workflow state machine.
/// </summary>
public class State : Entity<int>, IAuditable, ISoftDeletable
{
    public int WorkTypeId { get; private set; }
    public string SystemName { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public bool IsFinal { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public int? DeletedBy { get; private set; }

    private State() { }

    public State(int workTypeId, string systemName, int createdBy = 0, string? displayName = null, bool isFinal = false)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            throw new DomainException("State.SystemNameRequired", "System name is required.");

        WorkTypeId = workTypeId;
        SystemName = systemName;
        DisplayName = displayName ?? systemName;
        IsFinal = isFinal;

        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
    }

    /// <summary>
    /// Marks this state as a final state.
    /// </summary>
    public void MarkAsFinal(int modifiedBy)
    {
        IsFinal = true;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Updates the display name.
    /// </summary>
    public void UpdateDisplayName(string displayName, int modifiedBy)
    {
        DisplayName = displayName;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    /// <summary>
    /// Soft deletes the state.
    /// </summary>
    public void Delete(int deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}

/// <summary>
/// Well-known state names for Direction workflow.
/// </summary>
public static class DirectionStates
{
    public const string Draft = "DirectionDraft";
    public const string Submitted = "DirectionSubmitted";
    public const string Approved = "DirectionApproved";
    public const string Rejected = "DirectionRejected";
    public const string RequiresRevision = "DirectionRequiresRevision";
}

/// <summary>
/// Well-known state names for StudentWork workflow.
/// </summary>
public static class WorkStates
{
    // 1. Начало работы
    public const string Draft = "Draft";
    
    // 2. Предзащита 1 (Обязательная)
    public const string PreDefense1WaitingForFiles = "PreDefense1.WaitingForFiles";
    public const string PreDefense1WaitingForSchedule = "PreDefense1.WaitingForSchedule";
    public const string PreDefense1Scheduled = "PreDefense1.Scheduled";
    public const string PreDefense1Passed = "PreDefense1.Passed"; // Идет на Предзащиту 2
    public const string PreDefense1Failed = "PreDefense1.Failed"; // Идет на Предзащиту 2

    // 3. Предзащита 2 (Обязательная)
    public const string PreDefense2WaitingForFiles = "PreDefense2.WaitingForFiles";
    public const string PreDefense2WaitingForSchedule = "PreDefense2.WaitingForSchedule";
    public const string PreDefense2Scheduled = "PreDefense2.Scheduled";
    public const string PreDefense2Passed = "PreDefense2.Passed"; // Идет на Проверки (пропускает ПЗ-3)
    public const string PreDefense2Failed = "PreDefense2.Failed"; // Идет на Предзащиту 3

    // 4. Предзащита 3 (Последний шанс)
    public const string PreDefense3WaitingForFiles = "PreDefense3.WaitingForFiles";
    public const string PreDefense3WaitingForSchedule = "PreDefense3.WaitingForSchedule";
    public const string PreDefense3Scheduled = "PreDefense3.Scheduled";
    public const string PreDefense3Passed = "PreDefense3.Passed"; // Идет на Проверки
    public const string PreDefense3Failed = "PreDefense3.Failed"; // Недопуск (Отчисление/Отмена)

    // 5. Обязательные проверки (Пайплайн)
    public const string ChecksWaitingForInitial = "Checks.WaitingForInitial"; // Ждем нормоконтроль и доп. проверки кафедры
    public const string ChecksWaitingForAntiPlagiarism = "Checks.WaitingForAntiPlagiarism"; // Ждем антиплагиат
    public const string ReviewsWaitingForSupervisor = "Reviews.WaitingForSupervisor";
    public const string ReviewsWaitingForReviewer = "Reviews.WaitingForReviewer";

    // 6. Защита (ГАК)
    public const string ReadyForDefense = "ReadyForDefense";
    public const string DefenseWaitingForSchedule = "Defense.WaitingForSchedule";
    public const string DefenseScheduled = "Defense.Scheduled";
    public const string Defended = "Defended";
    public const string Graduated = "Graduated";
    public const string DefenseFailed = "Defense.Failed";

    // Отмена / Недопуск
    public const string Cancelled = "Cancelled";
}
