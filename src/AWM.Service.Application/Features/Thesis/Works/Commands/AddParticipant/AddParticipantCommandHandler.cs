namespace AWM.Service.Application.Features.Thesis.Works.Commands.AddParticipant;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for AddParticipantCommand.
/// Adds a new participant to an existing student work.
/// </summary>
public sealed class AddParticipantCommandHandler : IRequestHandler<AddParticipantCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddParticipantCommandHandler(
        IStudentWorkRepository workRepository,
        IUnitOfWork unitOfWork)
    {
        _workRepository = workRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(AddParticipantCommand request, CancellationToken cancellationToken)
    {
        // 1. Get work with participants loaded
        var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);

        if (work is null)
            return Result.Failure<long>(new Error("NotFound.Work", $"Student work with ID {request.WorkId} not found."));

        if (work.IsDeleted)
            return Result.Failure<long>(new Error("BusinessRule.WorkDeleted", "Cannot add participants to a deleted work."));

        // 2. Add participant (domain entity handles validation: max 5, no duplicates, single leader)
        try
        {
            var participant = work.AddParticipant(request.StudentId, request.Role);

            // 3. Persist
            await _workRepository.UpdateAsync(work, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(participant.Id);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<long>(new Error("BusinessRule.ParticipantError", ex.Message));
        }
    }
}
