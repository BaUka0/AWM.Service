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
        return Result.Failure(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}
