namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.UpdateAcademicProgram;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update an existing academic program.
/// </summary>
public sealed record UpdateAcademicProgramCommand : IRequest<Result>
{
    /// <summary>
    /// Academic program ID to update.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Program code (optional, e.g. "CS-B-2024").
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Program name (required, e.g. "Computer Science").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// User ID who modifies this program.
    /// </summary>
    public int ModifiedBy { get; init; }
}