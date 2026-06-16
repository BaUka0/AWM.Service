using AWM.Service.Application.Features.Defense.Schedules.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GetScheduleByWork;

public sealed record GetScheduleByWorkQuery(long WorkId) : IRequest<Result<ScheduleByWorkDto?>>;
