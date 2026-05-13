namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.DeleteAcademicProgram;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to soft-delete an academic program.
/// </summary>
public sealed record DeleteAcademicProgramCommand : IRequest<Result>
{
    public int Id { get; init; }
}
