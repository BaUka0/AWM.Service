namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new thesis topic.
/// Supervisor creates topics within approved directions or standalone.
/// </summary>
public sealed record CreateTopicCommand : IRequest<Result<long>>
{
    /// <summary>
    /// Department ID.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Supervisor ID (научный руководитель).
    /// </summary>
    public int SupervisorId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Work type ID (Diploma, Master's thesis, etc.).
    /// </summary>
    public int WorkTypeId { get; init; }

    /// <summary>
    /// Direction ID (optional - topic can be standalone or linked to direction).
    /// </summary>
    public long? DirectionId { get; init; }

    /// <summary>
    /// Topic title in Russian (required).
    /// </summary>
    public string TitleRu { get; init; } = null!;

    /// <summary>
    /// Topic title in Kazakh (optional).
    /// </summary>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Topic title in English (optional).
    /// </summary>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Topic description (optional).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Maximum number of participants (1-5 for team works).
    /// Default: 1 (individual work).
    /// </summary>
    public int MaxParticipants { get; init; } = 1;

    /// <summary>
    /// ID of the user creating the topic (for audit).
    /// </summary>
    public int CreatedBy { get; init; }
}