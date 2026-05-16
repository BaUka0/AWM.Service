namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsBySupervisor;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetTopicsBySupervisorQuery : IRequest<Result<IReadOnlyList<TopicDto>>>
{
    public int SupervisorId { get; init; }
    public int AcademicYearId { get; init; }
}
