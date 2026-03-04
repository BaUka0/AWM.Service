namespace AWM.Service.Application.Features.Thesis.Works.Commands.SetRepositoryUrl;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record SetRepositoryUrlCommand : IRequest<Result>
{
    public long WorkId { get; init; }
    public string RepositoryUrl { get; init; } = string.Empty;
}
