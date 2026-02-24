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
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyWorkQueryHandler(IStudentWorkRepository workRepository, ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository;
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

        var works = await _workRepository.GetByStudentAsync(_currentUserProvider.UserId.Value, cancellationToken);

        var dtos = works
            .Select(StudentWorkDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<StudentWorkDto>>(dtos);
    }
}
