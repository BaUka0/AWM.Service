namespace AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByStudent;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;
using AWM.Service.Domain.Repositories;
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

    public GetApplicationsByStudentQueryHandler(ITopicApplicationRepository applicationRepository)
    {
        _applicationRepository = applicationRepository;
    }

    public async Task<Result<IReadOnlyList<TopicApplicationDto>>> Handle(
        GetApplicationsByStudentQuery request, 
        CancellationToken cancellationToken)
    {
        // 1. Check authorization - students can only view their own applications
        // Note: In a real system, you might also allow admins/advisors
        if (request.StudentId != request.RequestingUserId)
        {
            return Result.Failure<IReadOnlyList<TopicApplicationDto>>(
                new Error("Authorization.Forbidden", "You can only view your own applications."));
        }

        // 2. Get applications
        IReadOnlyList<Domain.Thesis.Entities.TopicApplication> applications;

        if (request.AcademicYearId.HasValue)
        {
            // Get applications for specific academic year
            applications = await _applicationRepository.GetByStudentIdAndYearAsync(
                request.StudentId, 
                request.AcademicYearId.Value,
                cancellationToken);
        }
        else
        {
            // Get all applications
            applications = await _applicationRepository.GetByStudentIdAsync(
                request.StudentId, 
                cancellationToken);
        }

        // 3. Map to DTOs
        var dtos = applications
            .Select(TopicApplicationDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<TopicApplicationDto>>(dtos);
    }
}