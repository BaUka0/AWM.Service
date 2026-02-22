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

    public CreateStudentWorkCommandHandler(
        IStudentWorkRepository workRepository,
        ITopicRepository topicRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository;
        _topicRepository = topicRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<long>> Handle(CreateStudentWorkCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate topic if provided
        if (request.TopicId.HasValue)
        {
            var topic = await _topicRepository.GetByIdAsync(request.TopicId.Value, cancellationToken);

            if (topic is null)
                return Result.Failure<long>(new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));

            if (topic.IsDeleted)
                return Result.Failure<long>(new Error("BusinessRule.TopicDeleted", "Cannot create work for a deleted topic."));

            if (!topic.IsApproved)
                return Result.Failure<long>(new Error("BusinessRule.TopicNotApproved", "Cannot create work for an unapproved topic."));

            // Get the work type initial state ("Draft") from the topic's work type
            var draftState = await _workflowRepository.GetStateBySystemNameAsync(topic.WorkTypeId, "Draft", cancellationToken);

            if (draftState is null)
                return Result.Failure<long>(new Error("NotFound.State", "Draft state not found for this work type."));

            // 2. Create work
            var work = new StudentWork(
                academicYearId: request.AcademicYearId,
                departmentId: request.DepartmentId,
                draftStateId: draftState.Id,
                createdBy: _currentUserProvider.UserId!.Value,
                topicId: request.TopicId);

            // 3. Add the student as Leader
            work.AddParticipant(request.StudentId, ParticipantRole.Leader);

            // 4. Persist
            await _workRepository.AddAsync(work, cancellationToken);

            return Result.Success(work.Id);
        }
        else
        {
            // Standalone work (no topic) — use a default work type's Draft state
            // For now, we require a topic; standalone works could be implemented later
            return Result.Failure<long>(new Error("Validation.TopicRequired", "TopicId is required to create a student work."));
        }
    }
}
