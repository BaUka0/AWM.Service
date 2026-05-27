using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.DeleteCommission;

public sealed class DeleteCommissionCommandHandler : IRequestHandler<DeleteCommissionCommand, Result>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteCommissionCommandHandler(
        ICommissionRepository commissionRepository,
        IScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _commissionRepository = commissionRepository;
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(DeleteCommissionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var commission = await _commissionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (commission == null)
            return Result.Failure(new Error("Commission.NotFound", $"Commission with ID {request.Id} not found."));

        // Guard: cannot delete if students are already assigned to schedules
        var schedules = await _scheduleRepository.GetByCommissionAsync(request.Id, cancellationToken);
        if (schedules.Any(s => !s.IsDeleted))
            return Result.Failure(new Error("Commission.HasAssignedStudents",
                "Cannot delete a commission that has assigned students. Reassign or remove students first."));

        commission.Delete(_currentUserProvider.UserId.Value);
        await _commissionRepository.UpdateAsync(commission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
