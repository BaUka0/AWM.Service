namespace AWM.Service.Application.Features.Thesis.Applications.Commands.CreateApplication;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for CreateApplicationCommand.
/// Creates a new topic application for a student.
/// </summary>
public sealed class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Result<long>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ITopicApplicationRepository _applicationRepository;

    public CreateApplicationCommandHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository)
    {
        _topicRepository = topicRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<Result<long>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        // 1. Get topic
        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic is null)
        {
            return Result.Failure<long>(new Error("Topic.NotFound", $"Topic with ID {request.TopicId} not found."));
        }

        // 2. Check if topic is deleted
        if (topic.IsDeleted)
        {
            return Result.Failure<long>(new Error("Topic.Deleted", "Cannot apply to a deleted topic."));
        }

        // 3. Check if topic is approved
        if (!topic.IsApproved)
        {
            return Result.Failure<long>(new Error("Topic.NotApproved", "This topic is not yet approved for student applications."));
        }

        // 4. Check if topic is closed
        if (topic.IsClosed)
        {
            return Result.Failure<long>(new Error("Topic.Closed", "This topic is closed for applications."));
        }

        // 5. Check if topic can accept more applications
        if (!topic.CanAcceptApplications())
        {
            return Result.Failure<long>(new Error("Topic.Full", "This topic has reached maximum participants."));
        }

        // 6. Check if student already applied to this topic
        var hasApplied = await _applicationRepository.HasStudentAppliedToTopicAsync(
            request.StudentId, 
            request.TopicId, 
            cancellationToken);
        
        if (hasApplied)
        {
            return Result.Failure<long>(new Error("Application.Duplicate", "You have already applied to this topic."));
        }

        // 7. Optional: Check if student already has an accepted application this year
        // Uncomment if business rule requires only one accepted application per year
        /*
        var hasAccepted = await _applicationRepository.HasAcceptedApplicationAsync(
            request.StudentId,
            topic.AcademicYearId,
            cancellationToken);
        
        if (hasAccepted)
        {
            return Result.Failure<long>(new Error("Application.AlreadyAccepted", 
                "You already have an accepted application for this academic year."));
        }
        */

        // 8. Create application
        var application = new TopicApplication(
            topicId: request.TopicId,
            studentId: request.StudentId,
            motivationLetter: request.MotivationLetter
        );

        // 9. Add application to repository
        try
        {
            await _applicationRepository.AddAsync(application, cancellationToken);
            return Result.Success(application.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("Database.Error", $"Failed to create application: {ex.Message}"));
        }
    }
}