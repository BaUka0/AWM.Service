using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetTopicById;

public record GetTopicByIdQuery(long Id) : IRequest<Result<TopicDetailDto>>;
