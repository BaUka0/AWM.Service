namespace AWM.Service.Application.Features.Org.Institutes.Commands.UpdateInstitute;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update an existing Institute.
/// </summary>
public sealed record UpdateInstituteCommand : IRequest<Result>
{
    public int InstituteId { get; init; }
    public string Name { get; init; } = null!;
}