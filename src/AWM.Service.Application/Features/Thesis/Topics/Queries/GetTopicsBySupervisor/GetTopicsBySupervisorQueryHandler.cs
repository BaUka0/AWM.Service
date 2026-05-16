namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsBySupervisor;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetTopicsBySupervisorQueryHandler
    : IRequestHandler<GetTopicsBySupervisorQuery, Result<IReadOnlyList<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IUserRepository _userRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public GetTopicsBySupervisorQueryHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<Result<IReadOnlyList<TopicDto>>> Handle(
        GetTopicsBySupervisorQuery request,
        CancellationToken cancellationToken)
    {
        var topics = await _topicRepository.GetBySupervisorAsync(
            request.SupervisorId,
            request.AcademicYearId,
            cancellationToken);

        var dtos = new List<TopicDto>();
        foreach (var topic in topics.Where(t => !t.IsDeleted))
        {
            dtos.Add(await TopicDtoFactory.CreateAsync(
                topic,
                _directionRepository,
                _staffRepository,
                _userRepository,
                _workflowRepository,
                cancellationToken));
        }

        return Result.Success<IReadOnlyList<TopicDto>>(dtos);
    }
}
