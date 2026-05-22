namespace AWM.Service.Application.Features.Auth.Auth.Commands.UpdateRoleOperationAction;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class UpdateRoleOperationActionCommandHandler : IRequestHandler<UpdateRoleOperationActionCommand, Result>
{
    private readonly IRoleOperationActionRepository _roleOperationActionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleOperationActionCommandHandler(
        IRoleOperationActionRepository roleOperationActionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleOperationActionRepository = roleOperationActionRepository ?? throw new ArgumentNullException(nameof(roleOperationActionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(UpdateRoleOperationActionCommand request, CancellationToken cancellationToken)
    {
        var exists = await _roleOperationActionRepository.ExistsAsync(
            request.RoleAccessId,
            request.RoleOperationId,
            request.RoleActionTypeId,
            cancellationToken);

        if (request.IsGranted && !exists)
        {
            var action = new RoleOperationAction(request.RoleAccessId, request.RoleOperationId, request.RoleActionTypeId);
            await _roleOperationActionRepository.AddAsync(action, cancellationToken);
        }
        else if (!request.IsGranted && exists)
        {
            var existing = (await _roleOperationActionRepository.GetByRoleAccessIdAndOperationIdAsync(
                request.RoleAccessId, request.RoleOperationId, cancellationToken))
                .FirstOrDefault(a => a.RoleActionTypeId == request.RoleActionTypeId);

            if (existing != null)
            {
                await _roleOperationActionRepository.RemoveAsync(existing, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
