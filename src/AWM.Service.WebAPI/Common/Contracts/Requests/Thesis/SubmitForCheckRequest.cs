namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

/// <summary>
/// Request contract for submitting a work for quality check.
/// </summary>
public sealed record SubmitForCheckRequest
{
    /// <summary>
    /// Type of check to perform (1 = NormControl, 2 = SoftwareCheck, 3 = AntiPlagiarism).
    /// </summary>
    /// <example>2</example>
    public int CheckType { get; init; }

    /// <summary>
    /// Optional comment from the student.
    /// </summary>
    /// <example>Загружена финальная версия работы</example>
    public string? Comment { get; init; }
}
