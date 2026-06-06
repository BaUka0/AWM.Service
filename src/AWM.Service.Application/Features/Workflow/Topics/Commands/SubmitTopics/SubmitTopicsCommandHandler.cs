using System.Text.Json;
using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.SubmitTopics;

public sealed class SubmitTopicsCommandHandler : IRequestHandler<SubmitTopicsCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitTopicsCommandHandler(
        ITopicRepository topicRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ICurrentUserProvider currentUserProvider,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _currentUserProvider = currentUserProvider;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitTopicsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;
        var topics = await _topicRepository.GetByIdsAsync(request.TopicIds, cancellationToken);

        if (topics.Count != request.TopicIds.Count)
            return Result.Failure(new Error("Topics.NotFound", "Some topics were not found."));

        // Topics that will actually transition to Pending (not already Approved)
        var topicsToSubmit = topics.Where(t => t.Status != TopicStatus.Approved).ToList();
        if (!topicsToSubmit.Any())
            return Result.Success();

        // Validate MaxWorkload before submitting
        var firstTopic = topicsToSubmit.First();
        var supervisorAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit", firstTopic.OrgUnitId, StaffRoleType.Supervisor, cancellationToken);

        var assignment = supervisorAssignments
            .Where(a => a.IsActive && !a.IsDeleted && a.UserId == currentUserId)
            .FirstOrDefault(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                try
                {
                    var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == firstTopic.SemesterId && meta?.SpecialityId == firstTopic.SpecialityId;
                }
                catch { return false; }
            });

        if (assignment != null)
        {
            var metadata = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(assignment.MetadataJson!);
            int? maxWorkload = metadata?.MaxWorkload;

            if (maxWorkload.HasValue)
            {
                var existingTopics = await _topicRepository.GetBySupervisorAsync(currentUserId, firstTopic.SemesterId, cancellationToken);
                int currentTotal = existingTopics
                    .Where(t => t.Status == TopicStatus.Approved
                             || t.Status == TopicStatus.Pending
                             || t.Status == TopicStatus.Closed
                             || t.Status == TopicStatus.Reconciled)
                    .Sum(t => t.MaxParticipants);

                int submitTotal = topicsToSubmit.Sum(t => t.MaxParticipants);

                if (currentTotal + submitTotal > maxWorkload.Value)
                {
                    return Result.Failure(new Error("Supervisor.WorkloadExceeded",
                        $"Submitting these topics would exceed your MaxWorkload. Current: {currentTotal}, New: {submitTotal}, Max: {maxWorkload.Value}."));
                }
            }
        }

        foreach (var topic in topics)
        {
            if (topic.CreatedBy != currentUserId)
                return Result.Failure(new Error("Topics.Unauthorized", $"You are not authorized to submit topic ID {topic.Id}."));

            if (topic.Status == Domain.Thesis.Enums.TopicStatus.Approved)
                continue; // Already approved topics can be skipped

            // Validate stage for each topic (though they should share the same orgUnit/semester)
            var (isAllowed, errorMessage) = await _stageValidationService.ValidateOperationInStageAsync(
                topic.OrgUnitId,
                topic.SemesterId,
                WorkflowStageIds.TopicProposal,
                cancellationToken: cancellationToken);

            if (!isAllowed)
                return Result.Failure(new Error("Topics.StageClosed", $"Stage 4 is closed for topic ID {topic.Id}. {errorMessage}"));

            topic.SubmitForApproval();
            await _topicRepository.UpdateAsync(topic, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
