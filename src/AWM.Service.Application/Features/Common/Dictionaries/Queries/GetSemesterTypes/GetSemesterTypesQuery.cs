namespace AWM.Service.Application.Features.Common.Dictionaries.Queries.GetSemesterTypes;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.University;
using MediatR;

/// <summary>
/// Query to get all semester types (reference dictionary).
/// </summary>
public sealed record GetSemesterTypesQuery : IRequest<IReadOnlyList<SemesterType>>;

/// <summary>
/// Handler for GetSemesterTypesQuery.
/// </summary>
public sealed class GetSemesterTypesQueryHandler : IRequestHandler<GetSemesterTypesQuery, IReadOnlyList<SemesterType>>
{
    private readonly ISemesterTypeRepository _repository;

    public GetSemesterTypesQueryHandler(ISemesterTypeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyList<SemesterType>> Handle(GetSemesterTypesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
