namespace AWM.Service.Application.Features.Common.Commands.Periods.CreatePeriod;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class CreatePeriodCommandHandler : IRequestHandler<CreatePeriodCommand, Result<int>>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreatePeriodCommandHandler(
        IPeriodRepository periodRepository,
        IAcademicYearRepository academicYearRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
        _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreatePeriodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken);
            if (academicYear is null)
                return Result.Failure<int>(new Error("404", $"Academic year with ID {request.AcademicYearId} not found."));

            // Check for overlapping periods of the same stage in the same department/year
            var existingPeriods = await _periodRepository.GetByDepartmentAsync(request.DepartmentId, request.AcademicYearId, cancellationToken);
            var overlapping = existingPeriods
                .Where(p => !p.IsDeleted && p.WorkflowStage == request.WorkflowStage)
                .Any(p => request.StartDate < p.EndDate && request.EndDate > p.StartDate);

            if (overlapping)
                return Result.Failure<int>(new Error("409", "An overlapping period for this workflow stage already exists."));

            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<int>(new Error("401", "User ID is not available."));

            var period = new Period(
                request.DepartmentId,
                request.AcademicYearId,
                request.WorkflowStage,
                request.StartDate,
                request.EndDate,
                userId.Value);

            await _periodRepository.AddAsync(period, cancellationToken);
            return Result.Success(period.Id);
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<int>(new Error("500", $"An error occurred while creating the Period: {ex.Message}"));
        }
    }
}
