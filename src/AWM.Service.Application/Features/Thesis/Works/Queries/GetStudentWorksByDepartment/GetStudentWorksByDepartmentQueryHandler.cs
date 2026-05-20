namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorksByDepartment;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetStudentWorksByDepartmentQuery.
/// </summary>
public sealed class GetStudentWorksByDepartmentQueryHandler
    : IRequestHandler<GetStudentWorksByDepartmentQuery, Result<(IReadOnlyList<StudentWorkDto> Items, int TotalCount)>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetStudentWorksByDepartmentQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public async Task<Result<(IReadOnlyList<StudentWorkDto> Items, int TotalCount)>> Handle(
        GetStudentWorksByDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        int skip = (request.Page - 1) * request.PageSize;
        int take = request.PageSize;

        var (works, totalCount) = await _workRepository.GetByDepartmentPagedAsync(
            request.DepartmentId,
            request.AcademicYearId,
            skip,
            take,
            cancellationToken);

        var dtos = works
            .Select(StudentWorkDto.FromEntity)
            .ToList();

        return Result.Success<(IReadOnlyList<StudentWorkDto> Items, int TotalCount)>((dtos, totalCount));
    }
}
