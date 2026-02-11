namespace AWM.Service.Application.Features.Thesis.Topics.Commands.UpdateTopic;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating an existing thesis topic.
/// </summary>
public sealed class UpdateTopicCommandHandler : IRequestHandler<UpdateTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTopicCommandHandler(
        ITopicRepository topicRepository,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate input
            if (string.IsNullOrWhiteSpace(request.TitleRu))
            {
                return Result.Failure(new Error("Validation.Topic.TitleRequired", "Russian title is required."));
            }

            if (request.MaxParticipants < 1 || request.MaxParticipants > 5)
            {
                return Result.Failure(new Error("Validation.Topic.InvalidMaxParticipants", "Max participants must be between 1 and 5."));
            }

            // 2. Find the topic
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                return Result.Failure(new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));
            }

            // 3. Business rule: Cannot update approved topics
            if (topic.IsApproved)
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.CannotUpdateApproved", 
                    "Cannot update an approved topic. Please revoke approval first."));
            }

            // 4. Business rule: Cannot update closed topics
            if (topic.IsClosed)
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.CannotUpdateClosed", 
                    "Cannot update a closed topic. Please reopen the topic first."));
            }

            // 5. Business rule: If reducing MaxParticipants, check if it's possible
            var acceptedApplicationsCount = topic.Applications.Count(a => a.Status == Domain.Thesis.Enums.ApplicationStatus.Accepted);
            
            if (request.MaxParticipants < acceptedApplicationsCount)
            {
                return Result.Failure(new Error(
                    "BusinessRule.Topic.CannotReduceMaxParticipants", 
                    $"Cannot reduce max participants to {request.MaxParticipants}. There are already {acceptedApplicationsCount} accepted applications."));
            }

            // 6. Update topic content using domain methods
            topic.UpdateContent(
                request.TitleRu, 
                request.TitleKz, 
                request.TitleEn, 
                request.Description);

            // 7. Update max participants
            topic.UpdateMaxParticipants(request.MaxParticipants);

            // 8. Update metadata
            // Note: Domain entity should handle LastModifiedAt/LastModifiedBy automatically
            // If not, we'd need to add a method like topic.UpdateMetadata(modifiedBy)

            // 9. Save changes
            await _topicRepository.UpdateAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            // Domain validation errors
            return Result.Failure(new Error("Validation.Topic", argEx.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("InternalError", $"An error occurred while updating the topic: {ex.Message}"));
        }
    }
}