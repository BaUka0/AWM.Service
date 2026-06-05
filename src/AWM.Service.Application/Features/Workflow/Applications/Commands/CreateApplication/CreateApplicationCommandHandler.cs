using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.CreateApplication;

public sealed class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Result<long>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStudentReadOnlyRepository _studentReadOnlyRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        IStudentReadOnlyRepository studentReadOnlyRepository,
        ICurrentUserProvider currentUserProvider,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _studentReadOnlyRepository = studentReadOnlyRepository;
        _currentUserProvider = currentUserProvider;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure<long>(new Error("Auth.Unauthorized", "User is not authenticated."));

        var studentId = _currentUserProvider.UserId.Value;

        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic == null)
            return Result.Failure<long>(new Error("Topics.NotFound", "Topic not found."));

        if (topic.Status != Domain.Thesis.Enums.TopicStatus.Approved && topic.Status != Domain.Thesis.Enums.TopicStatus.Closed)
            return Result.Failure<long>(new Error("Topics.NotApproved", "Topic is not approved for selection."));

        if (topic.Status == Domain.Thesis.Enums.TopicStatus.Closed)
            return Result.Failure<long>(new Error("Topics.Closed", "Topic is closed for new applications."));

        // Validate Stage 5 (TopicSelection)
        var (isAllowed, errorMessage) = await _stageValidationService.ValidateOperationInStageAsync(
            topic.OrgUnitId,
            topic.SemesterId,
            WorkflowStageIds.TopicPreparation,
            cancellationToken: cancellationToken);

        if (!isAllowed)
            return Result.Failure<long>(new Error("Applications.StageClosed", errorMessage ?? "Topic selection period is closed."));

        // Check if already applied
        var alreadyApplied = await _applicationRepository.HasStudentAppliedToTopicAsync(studentId, request.TopicId, cancellationToken);
        if (alreadyApplied)
            return Result.Failure<long>(new Error("Applications.AlreadyApplied", "You have already applied to this topic."));

        // Get student's speciality
        var student = await _studentReadOnlyRepository.GetByUserIdAsync(studentId, cancellationToken);
        if (student == null)
            return Result.Failure<long>(new Error("Students.NotFound", "Student record not found."));

        // Create application
        var application = new TopicApplication(
            topicId: request.TopicId,
            studentId: studentId,
            motivationLetter: request.MotivationLetter,
            specialityId: student.SpecialityId);

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(application.Id);
    }
}
