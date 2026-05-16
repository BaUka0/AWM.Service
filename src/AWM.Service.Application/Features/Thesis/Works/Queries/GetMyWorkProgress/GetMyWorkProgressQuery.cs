namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWorkProgress;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetMyWorkProgressQuery : IRequest<Result<StudentWorkProgressDto?>>;
