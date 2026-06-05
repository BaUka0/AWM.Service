namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Check type reference entity.
/// Defines types of quality checks (e.g., Antiplagiarism, Software Check) that can be configured per department.
/// Maps to [Thesis].[CheckTypes].
/// </summary>
public class CheckType : Entity<int>
{
    public string Title { get; private set; } = null!;

    /// <summary>
    /// If true, this check type requires/supports a numeric result (e.g., percentage for Antiplagiarism).
    /// </summary>
    public bool HasNumericResult { get; private set; }

    /// <summary>
    /// Optional system code for hardcoded logic if absolutely necessary (e.g., "ANTIPLAGIARISM").
    /// </summary>
    public string? Code { get; private set; }

    private CheckType() { }

    public CheckType(int id, string title, bool hasNumericResult = false, string? code = null)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("CheckType.TitleRequired", "Title is required.");

        Title = title;
        HasNumericResult = hasNumericResult;
        Code = code;
    }

    public void Update(string title, bool hasNumericResult, string? code)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("CheckType.TitleRequired", "Title is required.");

        Title = title;
        HasNumericResult = hasNumericResult;
        Code = code;
    }
}
