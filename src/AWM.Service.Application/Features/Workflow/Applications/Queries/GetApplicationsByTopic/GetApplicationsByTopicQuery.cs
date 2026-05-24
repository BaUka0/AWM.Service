using AWM.Service.Application.Features.Workflow.Applications.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Queries.GetApplicationsByTopic;

public record GetApplicationsByTopicQuery(long TopicId) : IRequest<Result<List<TopicApplicationDto>>>;
