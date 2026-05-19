namespace AWM.Service.Application.Features.Edu.AcademicPrograms.Commands.CreateAcademicProgram;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.University;
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
        return Result.Failure<int>(new Error("NotImplemented", "Not implemented - University entities are read-only"));
    }
}