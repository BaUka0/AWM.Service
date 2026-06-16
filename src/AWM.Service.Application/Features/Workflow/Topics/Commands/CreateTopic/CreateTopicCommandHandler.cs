using System.Text.Json;
using AWM.Service.Application.Features.Workflow.Employees.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CreateTopic;

public sealed class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, Result<long>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IOrgUnitResolver _orgUnitResolver;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ICurrentUserProvider currentUserProvider,
        IOrgUnitResolver orgUnitResolver,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _currentUserProvider = currentUserProvider;
        _orgUnitResolver = orgUnitResolver;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<long>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var (resolvedOrgUnitId, orgUnitError) = await _orgUnitResolver.ResolveAsync(request.OrgUnitId, currentUserId, cancellationToken);
        if (!resolvedOrgUnitId.HasValue)
        {
            return Result.Failure<long>(new Error("OrgUnit.CannotResolve", orgUnitError ?? "Unable to determine department."));
        }

        var orgUnitId = resolvedOrgUnitId.Value;

        if (request.DirectionId.HasValue)
        {
            var direction = await _directionRepository.GetByIdAsync(request.DirectionId.Value, cancellationToken);
            if (direction == null)
            {
                return Result.Failure<long>(new Error("Topics.DirectionNotFound", "Specified direction not found."));
            }
        }

        var (isAllowed, errorMessage) = await _stageValidationService.ValidateOperationInStageAsync(
            orgUnitId,
            request.SemesterId,
            WorkflowStageIds.TopicProposal,
            cancellationToken: cancellationToken);

        if (!isAllowed)
        {
            return Result.Failure<long>(new Error("Topics.StageClosed", errorMessage ?? "The topic formation stage is closed."));
        }

        var supervisorAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "OrgUnit", orgUnitId, StaffRoleType.Supervisor, cancellationToken);

        var assignment = supervisorAssignments
            .Where(a => a.IsActive && !a.IsDeleted && a.UserId == currentUserId)
            .FirstOrDefault(a =>
            {
                if (string.IsNullOrEmpty(a.MetadataJson)) return false;
                try
                {
                    var meta = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(a.MetadataJson);
                    return meta?.SemesterId == request.SemesterId && meta?.SpecialityId == request.SpecialityId;
                }
                catch { return false; }
            });

        if (assignment != null)
        {
            var metadata = JsonSerializer.Deserialize<EmployeeAssignmentMetadata>(assignment.MetadataJson!);
            int? maxWorkload = metadata?.MaxWorkload;

                if (maxWorkload.HasValue)
                {
                    var existingTopics = await _topicRepository.GetBySupervisorAsync(currentUserId, request.SemesterId, cancellationToken);
                    int currentTotal = existingTopics
                        .Where(t => t.Status == TopicStatus.Approved
                                 || t.Status == TopicStatus.Pending
                                 || t.Status == TopicStatus.Closed
                                 || t.Status == TopicStatus.Reconciled)
                        .Sum(t => t.MaxParticipants);

                    if (currentTotal + request.MaxParticipants > maxWorkload.Value)
                    {
                        return Result.Failure<long>(new Error("Supervisor.WorkloadExceeded",
                            $"Creating this topic would exceed your MaxWorkload. Current: {currentTotal}, New: {request.MaxParticipants}, Max: {maxWorkload.Value}."));
                    }
                }
        }

        var topic = new Topic(
            orgUnitId: orgUnitId,
            createdByUserId: currentUserId,
            semesterId: request.SemesterId,
            workTypeId: request.WorkTypeId,
            titleRu: request.TitleRu,
            directionId: request.DirectionId,
            titleKz: request.TitleKz,
            titleEn: request.TitleEn,
            descriptionRu: request.DescriptionRu,
            descriptionKz: request.DescriptionKz,
            descriptionEn: request.DescriptionEn,
            maxParticipants: request.MaxParticipants,
            specialityId: request.SpecialityId);

        await _topicRepository.AddAsync(topic, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        topic.RaiseCreatedEvent();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(topic.Id);
    }
}
