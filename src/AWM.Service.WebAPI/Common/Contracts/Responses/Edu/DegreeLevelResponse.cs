namespace AWM.Service.WebAPI.Common.Contracts.Responses.Edu;

/// <summary>
/// Response contract for degree level.
/// </summary>
public sealed record DegreeLevelResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DurationYears { get; init; }

    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}