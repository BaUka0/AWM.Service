namespace AWM.Service.Application.Features.Admin.Users.Commands.ToggleUserStatus;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for ToggleUserStatusCommand.
/// Activates or deactivates the user based on the request.
/// </summary>
public sealed class ToggleUserStatusCommandHandler : IRequestHandler<ToggleUserStatusCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToggleUserStatusCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(new Error("NotFound.User", "Пользователь не найден."));

        if (request.IsActive)
            user.Activate();
        else
            user.Deactivate();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
