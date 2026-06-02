namespace AWM.Service.Application.Features.University.Queries.GetUsers;
using AWM.Service.Application.Features.University.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

public record GetUsersQuery(int? UniversityId) : IRequest<Result<IReadOnlyList<UserDto>>>;
