namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for updating an existing thesis topic.
/// </summary>
public sealed record UpdateTopicRequest
{
    /// <summary>
    /// Updated topic title in Russian.
    /// </summary>
    /// <example>Разработка системы управления дипломными проектами (обновлённая версия)</example>
    public string TitleRu { get; init; } = null!;

    /// <summary>
    /// Updated topic title in Kazakh (optional).
    /// </summary>
    /// <example>Дипломдық жобаларды басқару жүйесін әзірлеу (жаңартылған нұсқа)</example>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Updated topic title in English (optional).
    /// </summary>
    /// <example>Development of a Thesis Management System (Updated Version)</example>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Updated topic description (optional).
    /// </summary>
    /// <example>Обновлённое описание системы с новыми функциями</example>
    public string? Description { get; init; }

    /// <summary>
    /// Updated maximum number of participants (1-5).
    /// </summary>
    /// <example>2</example>
    public int MaxParticipants { get; init; }
}