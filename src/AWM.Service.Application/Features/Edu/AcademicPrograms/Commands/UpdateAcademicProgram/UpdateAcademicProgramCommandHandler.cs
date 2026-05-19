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
        return Result.Failure(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}