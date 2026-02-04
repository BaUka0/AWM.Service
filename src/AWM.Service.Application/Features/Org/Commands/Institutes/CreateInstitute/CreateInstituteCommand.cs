namespace AWM.Service.Application.Features.Org.Commands.Institutes.CreateInstitute;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new Institute within a University.
/// </summary>
public sealed record CreateInstituteCommand : IRequest<Result<int>>
{
    public int UniversityId { get; init; }
    public string Name { get; init; } = null!;
    public int CreatedBy { get; init; }
}