using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetAvailableTopics;

public record GetAvailableTopicsQuery(int OrgUnitId, int SemesterId) : IRequest<Result<List<TopicDto>>>;
