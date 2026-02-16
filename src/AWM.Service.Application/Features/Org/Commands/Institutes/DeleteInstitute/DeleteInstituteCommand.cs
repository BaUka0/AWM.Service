namespace AWM.Service.Application.Features.Org.Commands.Institutes.DeleteInstitute;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to soft delete an existing Institute.
/// </summary>
public sealed record DeleteInstituteCommand : IRequest<Result>
{
    public int InstituteId { get; init; }
}