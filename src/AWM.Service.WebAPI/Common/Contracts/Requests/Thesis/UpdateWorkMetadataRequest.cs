namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

public sealed record UpdateWorkMetadataRequest
{
    public string? MetadataJson { get; init; }
}
