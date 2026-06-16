namespace AWM.Service.Application.Features.University.Queries.GetSpecialityLevels;

using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public record GetSpecialityLevelsQuery() : IRequest<Result<IReadOnlyList<SpecialityLevelDto>>>;
