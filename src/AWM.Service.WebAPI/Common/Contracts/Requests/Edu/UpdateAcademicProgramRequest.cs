namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

/// <summary>
/// Request contract for updating an academic program.
/// </summary>
public sealed record UpdateAcademicProgramRequest
{
    /// <summary>
    /// Program code (optional, e.g. "CS-B-2024").
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Program name (required, e.g. "Computer Science").
    /// </summary>
    public string Name { get; init; } = string.Empty;
}