namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.UpdateAcademicProgram;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating an existing academic program.
/// </summary>
public sealed class UpdateAcademicProgramCommandHandler
    : IRequestHandler<UpdateAcademicProgramCommand, Result>
{
    private readonly IAcademicProgramRepository _academicProgramRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateAcademicProgramCommandHandler(
        IAcademicProgramRepository academicProgramRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _academicProgramRepository = academicProgramRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(
        UpdateAcademicProgramCommand request,
        CancellationToken cancellationToken)
    {
        // Get existing entity
        var academicProgram = await _academicProgramRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (academicProgram is null)
        {
            return Result.Failure(new Error(
                "404",
                $"Academic program with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (academicProgram.IsDeleted)
        {
            return Result.Failure(new Error(
                "409",
                $"Academic program with ID {request.Id} has been deleted."));
        }

        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        try
        {
            // Update using domain methods
            academicProgram.UpdateName(request.Name, userId.Value);
            academicProgram.UpdateCode(request.Code, userId.Value);

            // Save changes
            await _academicProgramRepository.UpdateAsync(academicProgram, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (from entity methods)
            return Result.Failure(new Error("400", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}