namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

public record UpdateStageRequest
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsActive { get; init; }
}
