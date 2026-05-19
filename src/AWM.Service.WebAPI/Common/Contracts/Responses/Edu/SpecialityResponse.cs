namespace AWM.Service.WebAPI.Common.Contracts.Responses.Edu;

/// <summary>
/// Response contract for university speciality.
/// </summary>
public sealed record SpecialityResponse
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? ShortTitle { get; init; }
    public int LevelId { get; init; }
    public int YearsOfStudy { get; init; }
    public bool IsDeleted { get; init; }
}
