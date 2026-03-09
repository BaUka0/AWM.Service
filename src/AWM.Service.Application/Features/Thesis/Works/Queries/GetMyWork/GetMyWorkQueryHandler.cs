namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWork;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetMyWorkQuery.
/// Returns all works where the student is a participant.
/// </summary>
public sealed class GetMyWorkQueryHandler
    : IRequestHandler<GetMyWorkQuery, Result<IReadOnlyList<StudentWorkDto>>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyWorkQueryHandler(
        IStudentWorkRepository workRepository,
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository;
        _studentRepository = studentRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<StudentWorkDto>>> Handle(
        GetMyWorkQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<StudentWorkDto>>(
                new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        // Resolve student profile — GetByStudentAsync expects Student.Id (FK to Edu.Students), not Auth.Users.Id
        var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken);
        if (student is null)
        {
            return Result.Failure<IReadOnlyList<StudentWorkDto>>(
                new Error("Authorization.Forbidden", "User does not have a student profile."));
        }

        var works = await _workRepository.GetByStudentAsync(student.Id, cancellationToken);

        var dtos = works
            .Select(StudentWorkDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<StudentWorkDto>>(dtos);
    }
}
