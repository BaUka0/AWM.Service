using AWM.Service.Application.Features.Workflow.Applications.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Queries.GetApplicationsByTopic;

public sealed class GetApplicationsByTopicQueryHandler : IRequestHandler<GetApplicationsByTopicQuery, Result<List<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;

    public GetApplicationsByTopicQueryHandler(ITopicApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<Result<List<TopicApplicationDto>>> Handle(GetApplicationsByTopicQuery request, CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetByTopicIdAsync(request.TopicId, cancellationToken);

        var dtos = applications.Select(a => new TopicApplicationDto(
            a.Id,
            a.TopicId,
            "", // TODO
            a.StudentId,
            "", // TODO
            "", // TODO
            a.MotivationLetter,
            GetStatus(a.StatusId),
            a.ReviewComment,
            a.AppliedAt,
            a.ReviewedAt
        )).ToList();

        return Result.Success(dtos);
    }

    private static string GetStatus(int statusId)
    {
        return statusId switch
        {
            (int)ApplicationStatusType.Submitted => "pending",
            (int)ApplicationStatusType.Accepted => "approved",
            (int)ApplicationStatusType.Rejected => "rejected",
            _ => "unknown"
        };
    }
}
