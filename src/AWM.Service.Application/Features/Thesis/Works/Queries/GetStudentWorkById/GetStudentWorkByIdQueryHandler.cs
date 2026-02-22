namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorkById;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetStudentWorkByIdQuery.
/// Returns full details of a student work including participants.
/// </summary>
public sealed class GetStudentWorkByIdQueryHandler
    : IRequestHandler<GetStudentWorkByIdQuery, Result<StudentWorkDetailDto>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetStudentWorkByIdQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public async Task<Result<StudentWorkDetailDto>> Handle(
        GetStudentWorkByIdQuery request,
        CancellationToken cancellationToken)
    {
        var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);

        if (work is null)
            return Result.Failure<StudentWorkDetailDto>(
                new Error("NotFound.Work", $"Student work with ID {request.WorkId} not found."));

        if (work.IsDeleted)
            return Result.Failure<StudentWorkDetailDto>(
                new Error("NotFound.Work", $"Student work with ID {request.WorkId} not found."));

        return Result.Success(StudentWorkDetailDto.FromEntity(work));
    }
}
