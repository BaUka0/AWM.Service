namespace AWM.Service.Application.Features.University.Queries.GetStudents;
using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public record GetStudentsQuery(string? Search, string? Status) : IRequest<Result<IReadOnlyList<StudentDto>>>;
