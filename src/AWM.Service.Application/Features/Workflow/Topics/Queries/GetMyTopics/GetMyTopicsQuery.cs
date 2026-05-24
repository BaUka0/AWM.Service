using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetMyTopics;

public record GetMyTopicsQuery(int SemesterId) : IRequest<Result<List<TopicDto>>>;
