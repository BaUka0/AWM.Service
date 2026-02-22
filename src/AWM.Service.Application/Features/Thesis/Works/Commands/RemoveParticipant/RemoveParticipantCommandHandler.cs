namespace AWM.Service.Application.Features.Thesis.Works.Commands.RemoveParticipant;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for RemoveParticipantCommand.
/// Removes a participant from a student work.
/// </summary>
public sealed class RemoveParticipantCommandHandler : IRequestHandler<RemoveParticipantCommand, Result>
{
    private readonly IStudentWorkRepository _workRepository;

    public RemoveParticipantCommandHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository;
    }

    public async Task<Result> Handle(RemoveParticipantCommand request, CancellationToken cancellationToken)
    {
        // 1. Get work with participants
        var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);

        if (work is null)
            return Result.Failure(new Error("NotFound.Work", $"Student work with ID {request.WorkId} not found."));

        if (work.IsDeleted)
            return Result.Failure(new Error("BusinessRule.WorkDeleted", "Cannot modify a deleted work."));

        // 2. Remove participant (domain entity handles validation)
        try
        {
            work.RemoveParticipant(request.StudentId);
            await _workRepository.UpdateAsync(work, cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("BusinessRule.ParticipantError", ex.Message));
        }
    }
}
