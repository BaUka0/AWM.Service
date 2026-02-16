namespace AWM.Service.Application.Features.Thesis.Topics.Commands.ApproveTopic;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for approving a thesis topic.
/// Once approved, the topic becomes available for student selection.
/// </summary>
public sealed class ApproveTopicCommandHandler : IRequestHandler<ApproveTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveTopicCommandHandler(
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(ApproveTopicCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Find the topic
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                return Result.Failure(new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));
            }

            // 2. Business rule: Cannot approve already approved topics
            if (topic.IsApproved)
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.AlreadyApproved", 
                    "This topic is already approved."));
            }

            // 3. Business rule: Cannot approve closed topics
            if (topic.IsClosed)
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.CannotApproveClosed", 
                    "Cannot approve a closed topic. Please reopen it first."));
            }

            // 4. Business rule: Topic must have valid content
            if (string.IsNullOrWhiteSpace(topic.TitleRu))
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.InvalidContent", 
                    "Topic must have a valid Russian title before approval."));
            }

            // 5. Approve topic using domain method
            topic.Approve();

            // Note: The domain entity raises TopicApprovedEvent which can be handled
            // by event handlers for notifications, etc.

            // 6. Save changes
            await _topicRepository.UpdateAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("InternalError", $"An error occurred while approving the topic: {ex.Message}"));
        }
    }
}