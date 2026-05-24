using AWM.Service.Application.Features.Workflow.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Queries.GetDepartmentTopics;

public record GetDepartmentTopicsQuery(int OrgUnitId, int SemesterId) : IRequest<Result<List<TopicDto>>>;
