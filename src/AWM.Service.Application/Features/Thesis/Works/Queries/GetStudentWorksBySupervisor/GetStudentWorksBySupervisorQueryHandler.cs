namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksBySupervisor;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetStudentWorksBySupervisorQuery.
/// </summary>
public sealed class GetStudentWorksBySupervisorQueryHandler
    : IRequestHandler<GetStudentWorksBySupervisorQuery, Result<IReadOnlyList<StudentWorkDto>>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetStudentWorksBySupervisorQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public async Task<Result<IReadOnlyList<StudentWorkDto>>> Handle(
        GetStudentWorksBySupervisorQuery request,
        CancellationToken cancellationToken)
    {
        var works = await _workRepository.GetBySupervisorAsync(
            request.SupervisorId,
            request.AcademicYearId,
            cancellationToken);

        var dtos = works
            .Select(StudentWorkDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<StudentWorkDto>>(dtos);
    }
}
