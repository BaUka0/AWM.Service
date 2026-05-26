using AWM.Service.Application.Features.Workflow.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Works.Queries.GetMySupervisedWorks;

public sealed record GetMySupervisedWorksQuery : IRequest<Result<IReadOnlyList<SupervisedWorkDto>>>;
