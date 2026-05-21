namespace AWM.Service.Application.Features.Thesis.Works.Commands.UpdateWorkMetadata;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record UpdateWorkMetadataCommand : IRequest<Result>
{
    public long WorkId { get; init; }
    public string? MetadataJson { get; init; }
}
