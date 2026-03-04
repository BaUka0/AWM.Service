namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

public sealed record SetRepositoryUrlRequest
{
    public string RepositoryUrl { get; init; } = string.Empty;
}
