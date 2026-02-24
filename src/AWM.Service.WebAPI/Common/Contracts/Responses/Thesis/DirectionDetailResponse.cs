namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for direction with full details.
/// </summary>
public sealed record DirectionDetailResponse : DirectionResponse
{
    public string? Description { get; init; }

    public string? ReviewComment { get; init; }

    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }

    public DateTime? DeletedAt { get; init; }
    public int? DeletedBy { get; init; }

    public int TopicsCount { get; init; }
}
