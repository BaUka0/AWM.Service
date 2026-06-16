using System.Text.Json;
using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.UpdateTopic;

public sealed class UpdateTopicCommandHandler : IRequestHandler<UpdateTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTopicCommandHandler(
        ITopicRepository topicRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var topic = await _topicRepository.GetByIdAsync(request.Id, cancellationToken);
        if (topic == null)
            return Result.Failure(new Error("Topics.NotFound", "Topic not found."));

        if (topic.CreatedBy != currentUserId)
            return Result.Failure(new Error("Topics.Unauthorized", "You can only update your own topics."));

        if (topic.Status == Domain.Thesis.Enums.TopicStatus.Approved)
            return Result.Failure(new Error("Topics.AlreadyApproved", "Approved topics cannot be updated."));

        if (request.MaxParticipants.HasValue && request.MaxParticipants.Value > topic.MaxParticipants)
        {
            var supervisorAssignments = await _staffAssignmentRepository.GetByRoleAsync(
                "OrgUnit", topic.OrgUnitId, StaffRoleType.Supervisor, cancellationToken);

            var assignment = supervisorAssignments
                .Where(a => a.IsActive && !a.IsDeleted && a.UserId == currentUserId)
                .FirstOrDefault(a =>
                {
                    if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                    try
                    {
                        var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson);
                        return meta?.SemesterId == topic.SemesterId && meta?.SpecialityId == topic.SpecialityId;
                    }
                    catch { return false; }
                });

            if (assignment != null)
            {
                var metadata = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(assignment.MetadataJson!);
                int? maxWorkload = metadata?.MaxWorkload;

                if (maxWorkload.HasValue)
                {
                    var existingTopics = await _topicRepository.GetBySupervisorAsync(currentUserId, topic.SemesterId, cancellationToken);
                    int currentTotal = existingTopics
                        .Where(t => t.Id != topic.Id)
                        .Where(t => t.Status == Domain.Thesis.Enums.TopicStatus.Approved
                                 || t.Status == Domain.Thesis.Enums.TopicStatus.Pending
                                 || t.Status == Domain.Thesis.Enums.TopicStatus.Closed
                                 || t.Status == Domain.Thesis.Enums.TopicStatus.Reconciled)
                        .Sum(t => t.MaxParticipants);

                    if (currentTotal + request.MaxParticipants.Value > maxWorkload.Value)
                    {
                        return Result.Failure(new Error("Supervisor.WorkloadExceeded",
                            $"Updating this topic would exceed your MaxWorkload. Current (other topics): {currentTotal}, New: {request.MaxParticipants.Value}, Max: {maxWorkload.Value}."));
                    }
                }
            }
        }

        topic.UpdateContent(
            request.TitleRu,
            request.TitleKz,
            request.TitleEn,
            request.DescriptionRu,
            request.DescriptionKz,
            request.DescriptionEn,
            request.MaxParticipants);

        await _topicRepository.UpdateAsync(topic, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
