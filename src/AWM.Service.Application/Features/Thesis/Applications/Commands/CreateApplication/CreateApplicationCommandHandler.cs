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

    public CreateApplicationCommandHandler(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
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
        var existingApplication = topic.Applications
            .FirstOrDefault(a => a.StudentId == request.StudentId && !a.IsDeleted);
        
        if (existingApplication is not null)
        {
            return Result.Failure<long>(new Error("Application.Duplicate", "You have already applied to this topic."));
        }

        // 7. Create application
        var application = new TopicApplication(
            topicId: request.TopicId,
            studentId: request.StudentId,
            motivationLetter: request.MotivationLetter
        );

        // 8. Add application to topic (assuming Topic has a method for this)
        // NOTE: If Topic doesn't have an AddApplication method, we need to add it to the domain entity
        // For now, we'll use reflection to add to private collection, but ideally this should be a domain method
        
        // TODO: Add this method to Topic entity:
        // public void AddApplication(TopicApplication application) { _applications.Add(application); }
        
        // Temporary workaround - directly accessing private field (NOT RECOMMENDED for production)
        // This should be replaced with a proper domain method
        var applicationsField = typeof(Topic).GetField("_applications", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (applicationsField is null)
        {
            return Result.Failure<long>(new Error("Internal.Error", "Failed to add application."));
        }

        var applications = applicationsField.GetValue(topic) as List<TopicApplication>;
        applications?.Add(application);

        // 9. Update topic
        try
        {
            await _topicRepository.UpdateAsync(topic, cancellationToken);
            return Result.Success(application.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("Database.Error", $"Failed to create application: {ex.Message}"));
        }
    }
}