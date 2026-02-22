namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMyWork;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Repositories;
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

    public GetMyWorkQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public async Task<Result<IReadOnlyList<StudentWorkDto>>> Handle(
        GetMyWorkQuery request,
        CancellationToken cancellationToken)
    {
        var works = await _workRepository.GetByStudentAsync(request.StudentId, cancellationToken);

        var dtos = works
            .Select(StudentWorkDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<StudentWorkDto>>(dtos);
    }
}
