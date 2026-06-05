namespace AWM.Service.Application.Features.University.Queries.GetSpecialities;

using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public record GetSpecialitiesQuery() : IRequest<Result<IReadOnlyList<SpecialityDto>>>;
