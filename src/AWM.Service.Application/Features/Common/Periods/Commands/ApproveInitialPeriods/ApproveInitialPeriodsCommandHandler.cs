namespace AWM.Service.Application.Features.Common.Periods.Commands.ApproveInitialPeriods;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.CommonDomain.Services;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class ApproveInitialPeriodsCommandHandler : IRequestHandler<ApproveInitialPeriodsCommand, Result>
{
    private readonly IPeriodRepository _periodRepository;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IAcademicProgramRepository _academicProgramRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;

    public ApproveInitialPeriodsCommandHandler(
        IPeriodRepository periodRepository,
        IAcademicYearRepository academicYearRepository,
        IStaffRepository staffRepository,
        IAcademicProgramRepository academicProgramRepository,
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
        _academicYearRepository = academicYearRepository ?? throw new ArgumentNullException(nameof(academicYearRepository));
        _staffRepository = staffRepository ?? throw new ArgumentNullException(nameof(staffRepository));
        _academicProgramRepository = academicProgramRepository ?? throw new ArgumentNullException(nameof(academicProgramRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public async Task<Result> Handle(ApproveInitialPeriodsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var academicYear = await _academicYearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken);
            if (academicYear is null)
                return Result.Failure(new Error("404", $"Academic year with ID {request.AcademicYearId} not found."));

            var existingPeriods = await _periodRepository.GetByDepartmentAsync(request.DepartmentId, request.AcademicYearId, cancellationToken);
            var activePeriods = existingPeriods.Where(p => !p.IsDeleted).ToList();

            foreach (var requestedPeriod in request.Periods)
            {
                var existing = activePeriods.FirstOrDefault(p => p.WorkflowStage == requestedPeriod.WorkflowStage);
                if (existing != null)
                {
                    existing.UpdateDates(requestedPeriod.StartDate, requestedPeriod.EndDate, userId.Value);
                    await _periodRepository.UpdateAsync(existing, cancellationToken);
                }
                else
                {
                    var newPeriod = new Period(
                        request.DepartmentId,
                        request.AcademicYearId,
                        requestedPeriod.WorkflowStage,
                        requestedPeriod.StartDate,
                        requestedPeriod.EndDate,
                        userId.Value);
                    await _periodRepository.AddAsync(newPeriod, cancellationToken);
                }
            }

            // 1. Notify Supervisors about Direction Submission
            var directionPeriod = request.Periods.FirstOrDefault(p => p.WorkflowStage == WorkflowStage.DirectionSubmission);
            if (directionPeriod != null)
            {
                var supervisors = await _staffRepository.GetSupervisorsWithCapacityAsync(request.DepartmentId, cancellationToken);
                var supervisorUserIds = supervisors.Select(s => s.UserId).ToList();

                if (supervisorUserIds.Any())
                {
                    var title = "Начало периода формирования направлений";
                    var body = $"Период формирования направлений и тем утвержден. Сроки: {directionPeriod.StartDate:dd.MM.yyyy} - {directionPeriod.EndDate:dd.MM.yyyy}. Желательно сформировать направления и темы в срок.";

                    await _notificationService.SendToManyAsync(
                        supervisorUserIds,
                        title,
                        userId.Value,
                        body,
                        cancellationToken: cancellationToken);
                }
            }

            // 2. Notify Students about Topic Selection
            var selectionPeriod = request.Periods.FirstOrDefault(p => p.WorkflowStage == WorkflowStage.TopicSelection);
            if (selectionPeriod != null)
            {
                var programs = await _academicProgramRepository.GetByDepartmentAsync(request.DepartmentId, cancellationToken);

                var studentUserIds = new List<int>();
                foreach (var program in programs)
                {
                    var students = await _studentRepository.GetByProgramAsync(program.Id, cancellationToken);
                    studentUserIds.AddRange(students.Select(s => s.UserId));
                }

                if (studentUserIds.Any())
                {
                    var title = "Сроки выбора тем дипломных работ";
                    var body = $"Внимание! Выбор тем дипломных будет проходить в период: {selectionPeriod.StartDate:dd.MM.yyyy} - {selectionPeriod.EndDate:dd.MM.yyyy}. Пожалуйста, осуществите выбор вовремя, иначе тема будет назначена случайным образом.";

                    await _notificationService.SendToManyAsync(
                        studentUserIds,
                        title,
                        userId.Value,
                        body,
                        cancellationToken: cancellationToken);
                }
            }

            return Result.Success();
        }
        catch (ArgumentException argEx)
        {
            return Result.Failure(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while approving the Periods: {ex.Message}"));
        }
    }
}
