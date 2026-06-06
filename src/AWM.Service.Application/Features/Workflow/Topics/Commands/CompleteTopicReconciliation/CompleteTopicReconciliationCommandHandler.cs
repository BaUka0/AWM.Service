using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Thesis.Events;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CompleteTopicReconciliation;

/// <summary>
/// Handles <see cref="CompleteTopicReconciliationCommand"/>.
/// 
/// Algorithm:
/// 1. Validate user has access to the orgUnit
/// 2. Load all reconciliation-eligible topics for the department/semester (optionally filtered by speciality)
/// 3. Validate there are no "hanging" topics (Approved/Closed that haven't been reconciled or marked inactive)
/// 4. For each Reconciled topic: create a StudentWork with its accepted students as WorkParticipants
/// 5. Raise TopicReconciliationCompletedEvent
/// </summary>
public sealed class CompleteTopicReconciliationCommandHandler
    : IRequestHandler<CompleteTopicReconciliationCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CompleteTopicReconciliationCommandHandler(
        ITopicRepository topicRepository,
        IStudentWorkRepository studentWorkRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IEmployeeReadOnlyRepository employeeRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _topicRepository = topicRepository;
        _studentWorkRepository = studentWorkRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CompleteTopicReconciliationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        // Validate user has access to the orgUnit via employee positions
        var employee = await _employeeRepository.GetByUserIdAsync(currentUserId, cancellationToken);
        var hasOrgUnitAccess = employee?.Positions.Any(p => p.OrgUnitId == request.OrgUnitId) ?? false;
        if (!hasOrgUnitAccess)
        {
            return Result.Failure(new Error(
                "Auth.OrgUnitAccessDenied",
                "You do not have access to this department."));
        }

        // 1. Load all topics for this department/semester with their applications
        var topics = await _topicRepository.GetByOrgUnitForReconciliationAsync(
            request.OrgUnitId, request.SemesterId, cancellationToken);

        // Apply optional speciality filter
        var filteredTopics = request.SpecialityId.HasValue
            ? topics.Where(t => t.SpecialityId == request.SpecialityId.Value).ToList()
            : topics.ToList();

        if (filteredTopics.Count == 0)
            return Result.Failure(new Error("Topics.NoTopicsFound", "No topics found for reconciliation in this department/semester."));

        // 2. Validate no "hanging" topics remain (Approved or Closed that weren't processed)
        var hangingTopics = filteredTopics
            .Where(t => t.Status == TopicStatus.Approved || t.Status == TopicStatus.Closed)
            .ToList();

        if (hangingTopics.Count > 0)
        {
            var hangingIds = string.Join(", ", hangingTopics.Select(t => t.Id));
            return Result.Failure(new Error(
                "Topics.UnprocessedTopics",
                $"Cannot complete reconciliation: {hangingTopics.Count} topic(s) still need to be reconciled, " +
                $"marked inactive, or sent back for revision. Topic IDs: {hangingIds}"));
        }

        // Also check for NeedsRevision — topics sent back to supervisors must be resolved first
        var revisionTopics = filteredTopics
            .Where(t => t.Status == TopicStatus.NeedsRevision)
            .ToList();

        if (revisionTopics.Count > 0)
        {
            var revisionIds = string.Join(", ", revisionTopics.Select(t => t.Id));
            return Result.Failure(new Error(
                "Topics.TopicsNeedRevision",
                $"Cannot complete reconciliation: {revisionTopics.Count} topic(s) are still awaiting supervisor revision. " +
                $"Topic IDs: {revisionIds}"));
        }

        // 3. Create StudentWork for each Reconciled topic
        var reconciledTopics = filteredTopics.Where(t => t.Status == TopicStatus.Reconciled).ToList();
        var anyWorkCreated = false;

        if (reconciledTopics.Count > 0)
        {
            // Get the first reconciled topic's WorkTypeId to look up the Draft state
            // NOTE: All topics in a department/semester should share the same workflow,
            // but we group by WorkTypeId to handle mixed types correctly
            var topicsByWorkType = reconciledTopics.GroupBy(t => t.WorkTypeId);

            foreach (var workTypeGroup in topicsByWorkType)
            {
                var draftState = await _workflowRepository.GetStateBySystemNameAsync(
                    workTypeGroup.Key, WorkStates.Draft, cancellationToken);

                if (draftState == null)
                    return Result.Failure(new Error(
                        "Workflow.DraftStateNotFound",
                        $"Draft state not found for work type {workTypeGroup.Key}. Workflow may not be configured."));

                foreach (var topic in workTypeGroup)
                {
                    var acceptedApplications = topic.Applications
                        .Where(a => a.StatusId == (int)ApplicationStatusType.Accepted)
                        .ToList();

                    if (acceptedApplications.Count == 0)
                        continue; // Skip reconciled topics with no accepted students (shouldn't happen due to domain validation)

                    // Prevent duplicate StudentWork creation
                    var alreadyExists = await _studentWorkRepository.ExistsByTopicIdAsync(topic.Id, cancellationToken);
                    if (alreadyExists)
                        continue;

                    // Create StudentWork entity
                    var work = new StudentWork(
                        semesterId: topic.SemesterId,
                        orgUnitId: topic.OrgUnitId,
                        draftStateId: draftState.Id,
                        createdBy: currentUserId,
                        topicId: topic.Id,
                        specialityId: topic.SpecialityId);

                    await _studentWorkRepository.AddAsync(work, cancellationToken);

                    // We need to save to get the generated Id before adding participants
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Add each accepted student as a participant
                    foreach (var application in acceptedApplications)
                    {
                        work.AddParticipant(application.StudentId, topic.MaxParticipants);
                    }

                    // Raise the created event now that we have a valid Id
                    work.RaiseCreatedEvent();

                    await _studentWorkRepository.UpdateAsync(work, cancellationToken);
                    anyWorkCreated = true;
                }
            }
        }

        // 4. Final save
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish the reconciliation completed event only if new works were actually created
        if (anyWorkCreated)
        {
            await _mediator.Publish(
                new TopicReconciliationCompletedEvent(request.OrgUnitId, request.SemesterId, currentUserId),
                cancellationToken);
        }

        return Result.Success();
    }
}

