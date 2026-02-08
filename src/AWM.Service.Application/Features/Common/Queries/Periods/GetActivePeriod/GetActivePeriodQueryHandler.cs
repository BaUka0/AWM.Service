namespace AWM.Service.Application.Features.Common.Queries.Periods.GetActivePeriod;

using AWM.Service.Application.Features.Common.DTOs;
using AWM.Service.Domain.Errors;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetActivePeriodQueryHandler : IRequestHandler<GetActivePeriodQuery, Result<PeriodDto?>>
{
    private readonly IPeriodRepository _periodRepository;

    public GetActivePeriodQueryHandler(IPeriodRepository periodRepository)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
    }

    public async Task<Result<PeriodDto?>> Handle(GetActivePeriodQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var period = await _periodRepository.GetActiveByStageAsync(
                request.DepartmentId,
                request.AcademicYearId,
                request.WorkflowStage,
                cancellationToken);

            if (period is null || period.IsDeleted)
                return Result.Success<PeriodDto?>(null);

            var dto = new PeriodDto
            {
                Id = period.Id,
                DepartmentId = period.DepartmentId,
                AcademicYearId = period.AcademicYearId,
                WorkflowStage = period.WorkflowStage.ToString(),
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                IsActive = period.IsActive,
                IsCurrentlyOpen = period.IsCurrentlyOpen(),
                CreatedAt = period.CreatedAt,
                CreatedBy = period.CreatedBy,
                LastModifiedAt = period.LastModifiedAt,
                LastModifiedBy = period.LastModifiedBy
            };

            return Result.Success<PeriodDto?>(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<PeriodDto?>(new Error(DomainErrors.General.InternalError, $"An error occurred: {ex.Message}"));
        }
    }
}
