namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWork;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all works for the current student ("My Works").
/// </summary>
public sealed record GetMyWorkQuery : IRequest<Result<IReadOnlyList<StudentWorkDto>>>
{
    /// <summary>
    /// ID of the student.
    /// </summary>
    public int StudentId { get; init; }
}
