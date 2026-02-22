namespace AWM.Service.Application.Features.Thesis.Applications.Commands.AcceptApplication;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for AcceptApplicationCommand.
/// Allows supervisor to accept a student's application to their topic.
/// </summary>
public sealed class AcceptApplicationCommandHandler : IRequestHandler<AcceptApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;

    public AcceptApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
    }

    public async Task<Result> Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
    {
        // 1. Get application with topic (for authorization)
        var application = await _applicationRepository.GetByIdWithTopicAsync(
            request.ApplicationId, 
            cancellationToken);
        
        if (application is null)
        {
            return Result.Failure(new Error("Application.NotFound", $"Application with ID {request.ApplicationId} not found."));
        }

        // 2. Check if application is deleted
        if (application.IsDeleted)
        {
            return Result.Failure(new Error("Application.Deleted", "Cannot accept a deleted application."));
        }

        // 3. Get the topic (we need it loaded separately for full checks)
        var topic = await _topicRepository.GetByIdAsync(application.TopicId, cancellationToken);
        if (topic is null)
        {
            return Result.Failure(new Error("Topic.NotFound", "Related topic not found."));
        }

        // 4. Check authorization - only the topic's supervisor can accept
        if (topic.SupervisorId != request.SupervisorId)
        {
            return Result.Failure(new Error("Authorization.Forbidden", "Only the topic supervisor can accept applications."));
        }

        // 5. Check if topic is still open for acceptance
        if (!topic.IsApproved)
        {
            return Result.Failure(new Error("Topic.NotApproved", "Cannot accept applications for an unapproved topic."));
        }

        if (topic.IsClosed)
        {
            return Result.Failure(new Error("Topic.Closed", "Cannot accept applications for a closed topic."));
        }

        if (topic.IsDeleted)
        {
            return Result.Failure(new Error("Topic.Deleted", "Cannot accept applications for a deleted topic."));
        }

        // 6. Check if there are available spots
        if (!topic.CanAcceptApplications())
        {
            return Result.Failure(new Error("Topic.Full", "This topic has reached maximum participants. Cannot accept more applications."));
        }

        // 7. Accept the application (domain method)
        try
        {
            application.Accept(request.SupervisorId);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("Application.InvalidState", ex.Message));
        }

        // 8. Update application
        try
        {
            await _applicationRepository.UpdateAsync(application, cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Database.Error", $"Failed to accept application: {ex.Message}"));
        }
    }
}