namespace AWM.Service.Application.Features.Edu.Commands.AcademicPrograms.CreateAcademicProgram;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new academic program.
/// </summary>
public sealed record CreateAcademicProgramCommand : IRequest<Result<int>>
{
    /// <summary>
    /// Department ID (foreign key).
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Degree level ID (Bachelor, Master, PhD).
    /// </summary>
    public int DegreeLevelId { get; init; }

    /// <summary>
    /// Program code (optional, e.g. "CS-B-2024").
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Program name (required, e.g. "Computer Science").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// User ID who creates this program.
    /// </summary>
    public int CreatedBy { get; init; }
}