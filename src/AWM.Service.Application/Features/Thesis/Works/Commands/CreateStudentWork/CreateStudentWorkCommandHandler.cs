namespace AWM.Service.Application.Features.Thesis.Works.Commands.CreateStudentWork;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for CreateStudentWorkCommand.
/// Creates a new student work, assigns the student as Leader, and sets the initial Draft state.
/// </summary>
public sealed class CreateStudentWorkCommandHandler : IRequestHandler<CreateStudentWorkCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentWorkCommandHandler(
        IStudentWorkRepository workRepository,
        ITopicRepository topicRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _workRepository = workRepository;
        _topicRepository = topicRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(CreateStudentWorkCommand request, CancellationToken cancellationToken)
    {
        // TopicId is required — standalone works without a topic are not supported yet
        if (!request.TopicId.HasValue)
            return Result.Failure<long>(new Error("Validation.TopicRequired", "TopicId is required to create a student work."));

        // 1. Validate current user
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<long>(new Error("Authorization.Unauthorized", "User identity could not be determined."));

        var topic = await _topicRepository.GetByIdAsync(request.TopicId.Value, cancellationToken);

        if (topic is null)
            return Result.Failure<long>(new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));

        if (topic.IsDeleted)
            return Result.Failure<long>(new Error("BusinessRule.TopicDeleted", "Cannot create work for a deleted topic."));

        if (!topic.IsApproved)
            return Result.Failure<long>(new Error("BusinessRule.TopicNotApproved", "Cannot create work for an unapproved topic."));

        // 2. Get the work type initial state ("Draft") from the topic's work type
        var draftState = await _workflowRepository.GetStateBySystemNameAsync(topic.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
            return Result.Failure<long>(new Error("NotFound.State", "Draft state not found for this work type."));

        // 3. Create work
        var work = new StudentWork(
            academicYearId: request.AcademicYearId,
            departmentId: request.DepartmentId,
            draftStateId: draftState.Id,
            createdBy: _currentUserProvider.UserId.Value,
            topicId: request.TopicId);

        // 4. Add the student as Leader
        work.AddParticipant(request.StudentId, ParticipantRole.Leader);

        // 5. Persist
        await _workRepository.AddAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(work.Id);
    }
}
