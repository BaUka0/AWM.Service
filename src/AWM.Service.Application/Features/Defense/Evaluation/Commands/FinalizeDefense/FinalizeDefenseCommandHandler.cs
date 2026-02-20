namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.FinalizeDefense;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for finalizing a defense.
/// Finds the protocol by schedule ID and calls Protocol.Finalize.
/// </summary>
public sealed class FinalizeDefenseCommandHandler : IRequestHandler<FinalizeDefenseCommand, Result>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public FinalizeDefenseCommandHandler(
        IProtocolRepository protocolRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _protocolRepository = protocolRepository ?? throw new ArgumentNullException(nameof(protocolRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(FinalizeDefenseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var protocol = await _protocolRepository.GetByScheduleIdAsync(request.ScheduleId, cancellationToken);
            if (protocol is null)
                return Result.Failure(new Error("NotFound.Protocol",
                    $"Protocol for schedule {request.ScheduleId} not found. Generate a protocol first."));

            protocol.Finalize(userId.Value);

            await _protocolRepository.UpdateAsync(protocol, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ioEx)
        {
            return Result.Failure(new Error("BusinessRule.Protocol", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}
