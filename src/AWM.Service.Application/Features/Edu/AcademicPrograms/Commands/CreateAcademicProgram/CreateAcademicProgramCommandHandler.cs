namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.CreateAcademicProgram;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new academic program.
/// </summary>
public sealed class CreateAcademicProgramCommandHandler
    : IRequestHandler<CreateAcademicProgramCommand, Result<int>>
{
    private readonly IAcademicProgramRepository _academicProgramRepository;
    private readonly IDegreeLevelRepository _degreeLevelRepository;
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateAcademicProgramCommandHandler(
        IAcademicProgramRepository academicProgramRepository,
        IDegreeLevelRepository degreeLevelRepository,
        IOrganizationLookupRepository organizationLookupRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _academicProgramRepository = academicProgramRepository;
        _degreeLevelRepository = degreeLevelRepository;
        _organizationLookupRepository = organizationLookupRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<int>> Handle(
        CreateAcademicProgramCommand request,
        CancellationToken cancellationToken)
    {
        // Validate department exists
        var department = await _organizationLookupRepository
            .GetDepartmentByIdAsync(request.DepartmentId, cancellationToken);

        if (department is null)
        {
            return Result.Failure<int>(new Error(
                "404",
                $"Department with ID {request.DepartmentId} not found."));
        }

        // Validate degree level exists
        var degreeLevel = await _degreeLevelRepository
            .GetByIdAsync(request.DegreeLevelId, cancellationToken);

        if (degreeLevel is null)
        {
            return Result.Failure<int>(new Error(
                "404",
                $"Degree level with ID {request.DegreeLevelId} not found."));
        }

        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure<int>(new Error("401", "User ID is not available."));
        }

        try
        {
            // Create entity using domain constructor
            var academicProgram = new AcademicProgram(
                departmentId: request.DepartmentId,
                degreeLevelId: request.DegreeLevelId,
                name: request.Name,
                createdBy: userId.Value,
                code: request.Code);

            // Save to repository
            await _academicProgramRepository.AddAsync(academicProgram, cancellationToken);

            return Result.Success(academicProgram.Id);
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (from entity constructor)
            return Result.Failure<int>(new Error("400", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}