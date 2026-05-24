using AWM.Service.Application.Features.Workflow.Applications.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Queries.GetMyApplications;

public sealed class GetMyApplicationsQueryHandler : IRequestHandler<GetMyApplicationsQuery, Result<List<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyApplicationsQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<List<TopicApplicationDto>>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<List<TopicApplicationDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var studentId = _currentUserProvider.UserId.Value;
        var applications = await _applicationRepository.GetByStudentIdAndYearAsync(studentId, request.SemesterId, cancellationToken);

        var dtos = applications.Select(a => new TopicApplicationDto(
            a.Id,
            a.TopicId,
            "", // TODO: Topic Title
            a.StudentId,
            "", // TODO: Student Name
            "", // TODO: Student Group
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
