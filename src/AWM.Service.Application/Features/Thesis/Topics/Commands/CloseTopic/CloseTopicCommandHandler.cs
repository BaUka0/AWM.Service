namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CloseTopic;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for closing a thesis topic.
/// Closed topics no longer accept new student applications.
/// </summary>
public sealed class CloseTopicCommandHandler : IRequestHandler<CloseTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseTopicCommandHandler(
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(CloseTopicCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Find the topic
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                return Result.Failure(new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));
            }

            // 2. Business rule: Cannot close already closed topics
            if (topic.IsClosed)
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.AlreadyClosed", 
                    "This topic is already closed."));
            }

            // 3. Business rule (optional): Topic must be approved before closing
            // This prevents closing draft topics that haven't been reviewed
            if (!topic.IsApproved)
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.CannotCloseUnapproved", 
                    "Cannot close an unapproved topic. Only approved topics can be closed."));
            }

            // 4. Business rule (optional): Check if all spots are filled
            // This is just a warning - supervisor can still close even if spots are available
            var acceptedCount = topic.Applications.Count(a => a.Status == Domain.Thesis.Enums.ApplicationStatus.Accepted);
            var isFull = acceptedCount >= topic.MaxParticipants;

            // Note: We could add a warning log here or return additional info
            // For now, we allow closing regardless of fill status

            // 5. Close topic using domain method
            topic.Close();

            // Note: The domain entity raises TopicClosedEvent which can be handled
            // by event handlers for notifications to students who applied, etc.

            // 6. Save changes
            await _topicRepository.UpdateAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("InternalError", $"An error occurred while closing the topic: {ex.Message}"));
        }
    }
}