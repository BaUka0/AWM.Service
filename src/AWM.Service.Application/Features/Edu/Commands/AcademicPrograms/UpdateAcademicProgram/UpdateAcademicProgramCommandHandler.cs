namespace AWM.Service.Application.Features.Edu.Commands.AcademicPrograms.UpdateAcademicProgram;

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

    public UpdateAcademicProgramCommandHandler(
        IAcademicProgramRepository academicProgramRepository)
    {
        _academicProgramRepository = academicProgramRepository;
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
                "AcademicProgram.NotFound", 
                $"Academic program with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (academicProgram.IsDeleted)
        {
            return Result.Failure(new Error(
                "AcademicProgram.Deleted", 
                $"Academic program with ID {request.Id} has been deleted."));
        }

        try
        {
            // Update using domain methods
            academicProgram.UpdateName(request.Name, request.ModifiedBy);
            academicProgram.UpdateCode(request.Code, request.ModifiedBy);

            // Save changes
            await _academicProgramRepository.UpdateAsync(academicProgram, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (from entity methods)
            return Result.Failure(new Error("Validation.Error", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("InternalError", ex.Message));
        }
    }
}