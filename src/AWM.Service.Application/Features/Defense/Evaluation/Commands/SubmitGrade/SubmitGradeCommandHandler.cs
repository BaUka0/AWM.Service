namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.SubmitGrade;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for a commission member submitting a grade for a defense slot.
/// Calls Schedule.AddGrade on the domain entity.
/// </summary>
public sealed class SubmitGradeCommandHandler : IRequestHandler<SubmitGradeCommand, Result<long>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitGradeCommandHandler(
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<long>> Handle(SubmitGradeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule is null)
                return Result.Failure<long>(new Error("NotFound.Schedule",
                    $"Schedule with ID {request.ScheduleId} not found."));

            if (schedule.IsDeleted)
                return Result.Failure<long>(new Error("BusinessRule.Schedule",
                    "Cannot submit a grade for a deleted schedule."));

            var grade = schedule.AddGrade(
                request.MemberId,
                request.CriteriaId,
                request.Score,
                request.Comment);

            await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(grade.Id);
        }
        catch (InvalidOperationException ioEx)
        {
            return Result.Failure<long>(new Error("BusinessRule.Grade", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
