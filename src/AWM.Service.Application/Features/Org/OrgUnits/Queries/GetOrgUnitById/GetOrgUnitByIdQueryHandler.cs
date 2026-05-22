namespace AWM.Service.Application.Features.Org.OrgUnits.Queries.GetOrgUnitById;

using AWM.Service.Application.Features.Org.OrgUnits.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for GetOrgUnitByIdQuery.
/// </summary>
public sealed class GetOrgUnitByIdQueryHandler : IRequestHandler<GetOrgUnitByIdQuery, Result<OrgUnitDto>>
{
    private readonly IOrgUnitReadOnlyRepository _repository;

    public GetOrgUnitByIdQueryHandler(IOrgUnitReadOnlyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<OrgUnitDto>> Handle(GetOrgUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure<OrgUnitDto>(new Error("404", $"OrgUnit with ID {request.Id} not found."));

        var dto = new OrgUnitDto
        {
            Id = entity.Id,
            ParentId = entity.ParentId,
            Name = entity.Title,
            Code = entity.ShortTitle,
            TypeId = entity.TypeId
        };

        return Result.Success(dto);
    }
}
