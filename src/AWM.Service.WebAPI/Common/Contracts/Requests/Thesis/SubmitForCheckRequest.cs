namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Request contract for submitting a work for quality check.
/// </summary>
public sealed record SubmitForCheckRequest
{
    /// <summary>
    /// Type of check to perform (0 = NormControl, 1 = SoftwareCheck, 2 = AntiPlagiarism).
    /// </summary>
    /// <example>2</example>
    public CheckType CheckType { get; init; }

    /// <summary>
    /// Optional comment from the student.
    /// </summary>
    /// <example>Загружена финальная версия работы</example>
    public string? Comment { get; init; }
}
