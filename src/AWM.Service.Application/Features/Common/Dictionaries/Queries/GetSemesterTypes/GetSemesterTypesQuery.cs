namespace AWM.Service.Application.Features.Common.Dictionaries.Queries.GetSemesterTypes;

using AWM.Service.Domain.University;
using MediatR;

/// <summary>
/// Query to get all semester types (reference dictionary).
/// </summary>
public sealed record GetSemesterTypesQuery : IRequest<IReadOnlyList<SemesterType>>;
