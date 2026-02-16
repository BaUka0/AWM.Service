namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for creating a new thesis topic.
/// </summary>
public sealed record CreateTopicRequest
{
    /// <summary>
    /// Department ID.
    /// </summary>
    /// <example>1</example>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Supervisor ID (научный руководитель).
    /// </summary>
    /// <example>5</example>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    /// <example>2</example>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Work type ID (Diploma, Master's thesis, etc.).
    /// </summary>
    /// <example>1</example>
    public int WorkTypeId { get; init; }

    /// <summary>
    /// Direction ID (optional - topic can be standalone or linked to direction).
    /// </summary>
    /// <example>10</example>
    public long? DirectionId { get; init; }

    /// <summary>
    /// Topic title in Russian (required).
    /// </summary>
    /// <example>Разработка системы управления дипломными проектами</example>
    public string TitleRu { get; init; } = null!;

    /// <summary>
    /// Topic title in Kazakh (optional).
    /// </summary>
    /// <example>Дипломдық жобаларды басқару жүйесін әзірлеу</example>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Topic title in English (optional).
    /// </summary>
    /// <example>Development of a Thesis Management System</example>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Topic description (optional).
    /// </summary>
    /// <example>Система для автоматизации процесса управления дипломными работами студентов</example>
    public string? Description { get; init; }

    /// <summary>
    /// Maximum number of participants (1-5 for team works). Default: 1.
    /// </summary>
    /// <example>1</example>
    public int MaxParticipants { get; init; } = 1;
}