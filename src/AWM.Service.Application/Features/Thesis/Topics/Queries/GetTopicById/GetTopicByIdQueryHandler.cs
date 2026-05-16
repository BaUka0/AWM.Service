namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicById;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a specific topic by ID with full details.
/// </summary>
public sealed class GetTopicByIdQueryHandler : IRequestHandler<GetTopicByIdQuery, Result<TopicDetailDto>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetTopicByIdQueryHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<Result<TopicDetailDto>> Handle(GetTopicByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Retrieve topic with applications
            var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);

            if (topic is null)
            {
                return Result.Failure<TopicDetailDto>(
                    new Error("NotFound.Topic", $"Topic with ID {request.TopicId} not found."));
            }

            var dto = await TopicDtoFactory.CreateDetailAsync(
                topic,
                _directionRepository,
                _staffRepository,
                _studentRepository,
                _userRepository,
                _workflowRepository,
                cancellationToken);

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<TopicDetailDto>(
                new Error("InternalError", $"An error occurred while retrieving the topic: {ex.Message}"));
        }
    }
}
