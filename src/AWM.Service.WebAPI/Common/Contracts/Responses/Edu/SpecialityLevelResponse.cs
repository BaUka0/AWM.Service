namespace AWM.Service.WebAPI.Common.Contracts.Responses.Edu;

/// <summary>
/// Response contract for university speciality level.
/// </summary>
public sealed record SpecialityLevelResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
