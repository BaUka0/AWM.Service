namespace AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByStudent;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all applications submitted by a student.
/// Used for student's "My Applications" page.
/// </summary>
public sealed record GetApplicationsByStudentQuery : IRequest<Result<IReadOnlyList<TopicApplicationDto>>>
{
    /// <summary>
    /// Optional: Filter by academic year.
    /// </summary>
    public int? AcademicYearId { get; init; }
}