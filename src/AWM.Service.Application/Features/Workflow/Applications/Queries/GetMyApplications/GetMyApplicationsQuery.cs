using AWM.Service.Application.Features.Workflow.Applications.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Queries.GetMyApplications;

public record GetMyApplicationsQuery(int SemesterId) : IRequest<Result<List<TopicApplicationDto>>>;
