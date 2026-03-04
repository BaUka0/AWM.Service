namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving topics available for student selection.
/// Returns only approved, open topics with available spots.
/// Auto-resolves DepartmentId and AcademicYearId from user context when not explicitly provided.
/// </summary>
public sealed class GetAvailableTopicsQueryHandler
    : IRequestHandler<GetAvailableTopicsQuery, Result<IReadOnlyList<TopicDto>>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAcademicYearRepository _academicYearRepository;

    public GetAvailableTopicsQueryHandler(
        ITopicRepository topicRepository,
        IUserRepository userRepository,
        IAcademicYearRepository academicYearRepository)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
    }

    public async Task<Result<IReadOnlyList<TopicDto>>> Handle(
        GetAvailableTopicsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Resolve DepartmentId — use explicit value or auto-resolve from user's role assignment context
            var departmentId = request.DepartmentId;
            if (!departmentId.HasValue && request.UserId.HasValue)
            {
                var user = await _userRepository.GetWithRoleAssignmentsAsync(request.UserId.Value, cancellationToken);
                departmentId = user?.RoleAssignments
                    .Where(ra => ra.IsCurrentlyValid() && ra.DepartmentId.HasValue)
                    .Select(ra => ra.DepartmentId)
                    .FirstOrDefault();
            }

            if (!departmentId.HasValue)
            {
                return Result.Failure<IReadOnlyList<TopicDto>>(
                    new Error("400", "Не удалось определить кафедру. Укажите departmentId или убедитесь, что у вас есть привязка к кафедре."));
            }

            // Resolve AcademicYearId — use explicit value or auto-resolve from current active year
            var academicYearId = request.AcademicYearId;
            if (!academicYearId.HasValue && request.UniversityId.HasValue)
            {
                var currentYear = await _academicYearRepository.GetCurrentAsync(request.UniversityId.Value, cancellationToken);
                academicYearId = currentYear?.Id;
            }

            if (!academicYearId.HasValue)
            {
                return Result.Failure<IReadOnlyList<TopicDto>>(
                    new Error("400", "Не удалось определить учебный год. Укажите academicYearId или убедитесь, что в системе задан текущий учебный год."));
            }

            // Retrieve available topics
            // This method already filters by: IsApproved = true, IsClosed = false, Available spots > 0
            var topics = await _topicRepository.GetAvailableForSelectionAsync(
                departmentId.Value,
                academicYearId.Value,
                cancellationToken);

            // Map to DTOs
            var dtos = topics.Select(t => new TopicDto
            {
                Id = t.Id,
                DirectionId = t.DirectionId,
                DepartmentId = t.DepartmentId,
                SupervisorId = t.SupervisorId,
                AcademicYearId = t.AcademicYearId,
                WorkTypeId = t.WorkTypeId,
                TitleRu = t.TitleRu,
                TitleEn = t.TitleEn,
                TitleKz = t.TitleKz,
                MaxParticipants = t.MaxParticipants,
                AvailableSpots = t.GetAvailableSpots(),
                IsApproved = t.IsApproved,
                IsClosed = t.IsClosed,
                IsTeamTopic = t.IsTeamTopic,
                CreatedAt = t.CreatedAt
            }).ToList();

            return Result.Success<IReadOnlyList<TopicDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<TopicDto>>(
                new Error("InternalError", $"An error occurred while retrieving available topics: {ex.Message}"));
        }
    }
}