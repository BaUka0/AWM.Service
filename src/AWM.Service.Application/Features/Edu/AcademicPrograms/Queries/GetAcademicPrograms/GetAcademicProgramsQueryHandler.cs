namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Queries.GetAcademicPrograms;

using AWM.Service.Application.Features.Edu.AcademicPrograms.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving academic programs with filtering.
/// </summary>
public sealed class GetAcademicProgramsQueryHandler 
    : IRequestHandler<GetAcademicProgramsQuery, Result<IReadOnlyList<AcademicProgramDto>>>
{
    private readonly IAcademicProgramRepository _academicProgramRepository;

    public GetAcademicProgramsQueryHandler(
        IAcademicProgramRepository academicProgramRepository)
    {
        _academicProgramRepository = academicProgramRepository;
    }

    public async Task<Result<IReadOnlyList<AcademicProgramDto>>> Handle(
        GetAcademicProgramsQuery request, 
        CancellationToken cancellationToken)
    {
        return Result.Failure<IReadOnlyList<AcademicProgramDto>>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}