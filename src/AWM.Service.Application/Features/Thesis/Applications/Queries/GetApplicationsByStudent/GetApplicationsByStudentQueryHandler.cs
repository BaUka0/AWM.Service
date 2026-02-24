namespace AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByStudent;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetApplicationsByStudentQuery.
/// Retrieves all applications submitted by a student with authorization check.
/// </summary>
public sealed class GetApplicationsByStudentQueryHandler
    : IRequestHandler<GetApplicationsByStudentQuery, Result<IReadOnlyList<TopicApplicationDto>>>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetApplicationsByStudentQueryHandler(
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<TopicApplicationDto>>> Handle(
        GetApplicationsByStudentQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        // 1. Get applications
        IReadOnlyList<Domain.Thesis.Entities.TopicApplication> applications;

        if (request.AcademicYearId.HasValue)
        {
            // Get applications for specific academic year
            applications = await _applicationRepository.GetByStudentIdAndYearAsync(
                userId,
                request.AcademicYearId.Value,
                cancellationToken);
        }
        else
        {
            // Get all applications
            applications = await _applicationRepository.GetByStudentIdAsync(
                userId,
                cancellationToken);
        }

        // 3. Map to DTOs
        var dtos = applications
            .Select(TopicApplicationDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<TopicApplicationDto>>(dtos);
    }
}