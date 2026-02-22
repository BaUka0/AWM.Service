namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksByDepartment;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetStudentWorksByDepartmentQuery.
/// </summary>
public sealed class GetStudentWorksByDepartmentQueryHandler
    : IRequestHandler<GetStudentWorksByDepartmentQuery, Result<IReadOnlyList<StudentWorkDto>>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetStudentWorksByDepartmentQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public async Task<Result<IReadOnlyList<StudentWorkDto>>> Handle(
        GetStudentWorksByDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        var works = await _workRepository.GetByDepartmentAsync(
            request.DepartmentId,
            request.AcademicYearId,
            cancellationToken);

        var dtos = works
            .Select(StudentWorkDto.FromEntity)
            .ToList();

        return Result.Success<IReadOnlyList<StudentWorkDto>>(dtos);
    }
}
