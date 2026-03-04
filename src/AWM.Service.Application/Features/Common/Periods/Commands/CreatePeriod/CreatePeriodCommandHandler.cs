namespace AWM.Service.Application.Features.Common.Periods.Commands.CreatePeriod;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class CreatePeriodCommandHandler : IRequestHandler<CreatePeriodCommand, Result<int>>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePeriodCommandHandler> _logger;

    public CreatePeriodCommandHandler(
        IPeriodRepository periodRepository,
        IAcademicYearRepository academicYearRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<CreatePeriodCommandHandler> logger)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
        _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(CreatePeriodCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            _logger.LogInformation("Attempting to create period for Dept={DeptId}, Year={YearId}, Stage={Stage} by User={UserId}",
                request.DepartmentId, request.AcademicYearId, request.WorkflowStage, userId);

            var academicYear = await _academicYearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken);
            if (academicYear is null)
            {
                _logger.LogWarning("CreatePeriod failed: Academic year {YearId} not found.", request.AcademicYearId);
                return Result.Failure<int>(new Error("404", $"Academic year with ID {request.AcademicYearId} not found."));
            }

            // Check for overlapping periods of the same stage in the same department/year
            var existingPeriods = await _periodRepository.GetByDepartmentAsync(request.DepartmentId, request.AcademicYearId, cancellationToken);
            var overlapping = existingPeriods
                .Where(p => !p.IsDeleted && p.WorkflowStage == request.WorkflowStage)
                .Any(p => request.StartDate < p.EndDate && request.EndDate > p.StartDate);

            if (overlapping)
            {
                _logger.LogWarning("CreatePeriod failed: Overlapping period for Stage={Stage} in Dept={DeptId}, Year={YearId}",
                    request.WorkflowStage, request.DepartmentId, request.AcademicYearId);
                return Result.Failure<int>(new Error("409", "An overlapping period for this workflow stage already exists."));
            }

            if (!userId.HasValue)
            {
                _logger.LogWarning("CreatePeriod failed: User ID is not available.");
                return Result.Failure<int>(new Error("401", "User ID is not available."));
            }

            var period = new Period(
                request.DepartmentId,
                request.AcademicYearId,
                request.WorkflowStage,
                request.StartDate,
                request.EndDate,
                userId.Value);

            await _periodRepository.AddAsync(period, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created period with ID={PeriodId} for Dept={DeptId}", period.Id, request.DepartmentId);
            return Result.Success(period.Id);
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning(argEx, "CreatePeriod validation failed: {Message}", argEx.Message);
            return Result.Failure<int>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreatePeriod failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure<int>(new Error("500", $"An error occurred while creating the Period: {ex.Message}"));
        }
    }
}
