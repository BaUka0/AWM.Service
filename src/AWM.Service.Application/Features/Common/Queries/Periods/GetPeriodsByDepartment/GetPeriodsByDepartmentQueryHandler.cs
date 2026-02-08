namespace AWM.Service.Application.Features.Common.Queries.Periods.GetPeriodsByDepartment;

using AWM.Service.Application.Features.Common.DTOs;
using AWM.Service.Domain.Errors;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetPeriodsByDepartmentQueryHandler : IRequestHandler<GetPeriodsByDepartmentQuery, Result<IReadOnlyList<PeriodDto>>>
{
    private readonly IPeriodRepository _periodRepository;

    public GetPeriodsByDepartmentQueryHandler(IPeriodRepository periodRepository)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
    }

    public async Task<Result<IReadOnlyList<PeriodDto>>> Handle(GetPeriodsByDepartmentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var periods = await _periodRepository.GetByDepartmentAsync(request.DepartmentId, request.AcademicYearId, cancellationToken);

            var dtos = periods
                .Where(p => !p.IsDeleted)
                .Select(p => new PeriodDto
                {
                    Id = p.Id,
                    DepartmentId = p.DepartmentId,
                    AcademicYearId = p.AcademicYearId,
                    WorkflowStage = p.WorkflowStage.ToString(),
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive,
                    IsCurrentlyOpen = p.IsCurrentlyOpen(),
                    CreatedAt = p.CreatedAt,
                    CreatedBy = p.CreatedBy,
                    LastModifiedAt = p.LastModifiedAt,
                    LastModifiedBy = p.LastModifiedBy
                })
                .ToList();

            return Result.Success<IReadOnlyList<PeriodDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<PeriodDto>>(new Error(DomainErrors.General.InternalError, $"An error occurred: {ex.Message}"));
        }
    }
}
