namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.DeleteAcademicProgram;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for DeleteAcademicProgramCommand.
/// </summary>
public sealed class DeleteAcademicProgramCommandHandler : IRequestHandler<DeleteAcademicProgramCommand, Result>
{
    private readonly IAcademicProgramRepository _eduRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteAcademicProgramCommandHandler(
        IAcademicProgramRepository eduRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _eduRepository = eduRepository ?? throw new ArgumentNullException(nameof(eduRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(DeleteAcademicProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _eduRepository.GetByIdAsync(request.Id, cancellationToken);

        if (program is null)
        {
            return Result.Failure(new Error("NotFound.AcademicProgram", $"Academic program with ID {request.Id} not found."));
        }

        if (program.IsDeleted)
        {
            return Result.Failure(new Error("Conflict.AcademicProgram", "Academic program is already deleted."));
        }

        program.Delete(_currentUserProvider.UserId ?? 0);
        await _eduRepository.UpdateAsync(program, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
