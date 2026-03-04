namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve topics available for student selection.
/// Returns approved, open topics with available spots.
/// When DepartmentId or AcademicYearId are not provided, the handler resolves
/// them automatically from the current user's JWT context.
/// </summary>
public sealed record GetAvailableTopicsQuery : IRequest<Result<IReadOnlyList<TopicDto>>>
{
    /// <summary>
    /// Department ID to filter topics.
    /// When null, auto-resolved from the authenticated user's role assignment.
    /// </summary>
    public int? DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID to filter topics.
    /// When null, auto-resolved using the currently active academic year.
    /// </summary>
    public int? AcademicYearId { get; init; }

    /// <summary>
    /// The ID of the currently authenticated user.
    /// Used for auto-resolving DepartmentId and AcademicYearId when not provided.
    /// </summary>
    public int? UserId { get; init; }

    /// <summary>
    /// University ID used for resolving the current academic year.
    /// </summary>
    public int? UniversityId { get; init; }
}