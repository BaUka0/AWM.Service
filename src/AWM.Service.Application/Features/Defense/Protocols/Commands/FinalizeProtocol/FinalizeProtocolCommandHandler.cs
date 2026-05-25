using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Commands.FinalizeProtocol;

public sealed class FinalizeProtocolCommandHandler : IRequestHandler<FinalizeProtocolCommand, Result>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public FinalizeProtocolCommandHandler(
        IProtocolRepository protocolRepository,
        ICommissionRepository commissionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _protocolRepository = protocolRepository;
        _commissionRepository = commissionRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(FinalizeProtocolCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var protocol = await _protocolRepository.GetByIdAsync(request.ProtocolId, cancellationToken);
        if (protocol == null)
            return Result.Failure(new Error("Protocol.NotFound", $"Protocol with ID {request.ProtocolId} not found."));

        // Only the commission chairman or secretary may finalize
        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(protocol.CommissionId, cancellationToken);
        if (commission == null)
            return Result.Failure(new Error("Commission.NotFound", "Commission for this protocol not found."));

        var userAssignment = commission.Assignments
            .FirstOrDefault(a => a.UserId == currentUserId && a.IsActive && !a.IsDeleted);

        if (userAssignment == null ||
            (userAssignment.RoleType != StaffRoleType.CommissionChairman &&
             userAssignment.RoleType != StaffRoleType.CommissionSecretary))
        {
            return Result.Failure(new Error("Commission.Unauthorized",
                "Only the chairman or secretary of the commission can finalize the protocol."));
        }

        try
        {
            protocol.Finalize(currentUserId);

            await _protocolRepository.UpdateAsync(protocol, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(new Error(ex.ErrorCode, ex.Message));
        }
    }
}
