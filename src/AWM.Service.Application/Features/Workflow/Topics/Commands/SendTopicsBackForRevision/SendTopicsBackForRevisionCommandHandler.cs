using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.SendTopicsBackForRevision;

/// <summary>
/// Handles <see cref="SendTopicsBackForRevisionCommand"/>.
/// Sends topics back to supervisors for revision — typically when excess applications
/// need supervisor resolution. Validates user has orgUnit access before processing.
/// Sets status to NeedsRevision with a comment.
/// </summary>
public sealed class SendTopicsBackForRevisionCommandHandler : IRequestHandler<SendTopicsBackForRevisionCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendTopicsBackForRevisionCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IEmployeeReadOnlyRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SendTopicsBackForRevisionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;
        var topics = await _topicRepository.GetByIdsAsync(request.TopicIds, cancellationToken);

        if (topics.Count != request.TopicIds.Count)
        {
            var foundIds = topics.Select(t => t.Id).ToHashSet();
            var missingIds = request.TopicIds.Where(id => !foundIds.Contains(id)).ToList();
            return Result.Failure(new Error("Topics.NotFound", $"Topics not found: {string.Join(", ", missingIds)}"));
        }

        var orgUnitId = topics.First().OrgUnitId;
        if (topics.Any(t => t.OrgUnitId != orgUnitId))
        {
            return Result.Failure(new Error("Topics.MultipleOrgUnits", "All topics must belong to the same department."));
        }

        var employee = await _employeeRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        var hasOrgUnitAccess = employee?.Positions.Any(p => p.OrgUnitId == orgUnitId) ?? false;
        if (!hasOrgUnitAccess)
        {
            return Result.Failure(new Error(
                "Auth.OrgUnitAccessDenied",
                "You do not have access to this department."));
        }

        foreach (var topic in topics)
        {
            topic.SendBackForRevision(currentUserId, request.Comment);
            await _topicRepository.UpdateAsync(topic, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
